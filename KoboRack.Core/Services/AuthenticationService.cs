using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using KoboRack.Core.DTO;
using KoboRack.Core.IServices;
using KoboRack.Data.Context;
using KoboRack.Model;
using KoboRack.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KoboRack.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailServices _emailServices;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger _logger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailServices _emailService;
        private readonly IWalletServices services;
        private readonly SaviDbContext _saviDbContext;
        private readonly ICloudinaryServices<AppUser> _cloudinaryServices;

        public AuthenticationService(IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<EmailSettings> emailSettings, ILogger<AuthenticationService> logger, IEmailServices emailService, IWalletServices services, SaviDbContext saviDbContext, ICloudinaryServices<AppUser> cloudinaryServices)
        {
            _config = config;
            _userManager = userManager;
            _emailServices = new EmailServices(emailSettings);
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _signInManager = signInManager;
            _emailService = emailService;
            this.services = services;
            _saviDbContext = saviDbContext;
            _cloudinaryServices = cloudinaryServices;
        }
        public async Task<ApiResponse<string>> LoginAsync(AppUserLoginDTO loginDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                    return ApiResponse<string>.Failed(false, "Invalid Email or password.", 400, new List<string> { "Invalid Email or password." });
                if (!user.EmailConfirmed)
                {
                    return ApiResponse<string>.Failed(false, "You have not confirmed your email", 400, new List<string> { "Please, confirm your email and Login again." });

                }
                if (await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                {
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role),
            };
                    var jwtToken = GetToken(authClaims);
                    return ApiResponse<string>.Success(new JwtSecurityTokenHandler().WriteToken(jwtToken), "Login successful", 200);
                }
                return ApiResponse<string>.Failed(false, "Invalid Email or password.", 400, new List<string> { "Invalid Email or password." });
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failed(false, "An unexpected error occurred during login.", 500, new List<string> { ex.Message });
            }
        }
        public JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:ValidIssuer"],
                audience: _config["JwtSettings:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }
        public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found or email not confirmed.", StatusCodes.Status404NotFound, null, new List<string>());
                }
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                user.PasswordResetToken = token;
                user.ResetTokenExpires = DateTime.UtcNow.AddHours(24);

                await _userManager.UpdateAsync(user);

                var resetPasswordUrl = "http://localhost:3000/resetpassword?email=" + Uri.EscapeDataString(email) + "&token=" + Uri.EscapeDataString(token);

                var mailRequest = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Savi Password Reset Instructions",
                    Body = $"Please reset your password by clicking <a href='{resetPasswordUrl}'>here</a>."
                };
                await _emailServices.SendHtmlEmailAsync(mailRequest);

                return new ApiResponse<string>(true, "Password reset email sent successfully.", 200, null, new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resolving password change");
                var errorList = new List<string>();
                errorList.Add(ex.Message);
                return new ApiResponse<string>(true, "Error occurred while resolving password change", 500, null, errorList);
            }
        }
        public async Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found.", 404, null, new List<string>());
                }
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    user.PasswordResetToken = null;
                    user.ResetTokenExpires = null;

                    await _userManager.UpdateAsync(user);

                    return new ApiResponse<string>(true, "Password reset successful.", 200, null, new List<string>());
                }
                else
                {
                    return new ApiResponse<string>(false, "Password reset failed.", 400, null, result.Errors.Select(error => error.Description).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting password");
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(true, "Error occurred while resetting password", 500, null, errorList);
            }
        }
        public async Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            try
            {
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (result.Succeeded)
                {
                    return new ApiResponse<string>(true, "Password changed successfully.", 200, null, new List<string>());
                }
                else
                {
                    return new ApiResponse<string>(false, "Password change failed.", 400, null, result.Errors.Select(error => error.Description).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password");
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(true, "Error occurred while changing password", 500, null, errorList);
            }
        }
        public async Task<ApiResponse<string>> RegisterAsync(AppUserCreateDto appUserCreateDto)
        {
            var validationResults = new List<ValidationResult>();
            var isValidModel = Validator.TryValidateObject(appUserCreateDto, new ValidationContext(appUserCreateDto), validationResults, true);

            if (!isValidModel)
            {
                var errorMessages = validationResults.Select(r => r.ErrorMessage).ToList();
                return new ApiResponse<string>(false, "Invalid input data.", StatusCodes.Status400BadRequest, errorMessages);
            }

            if (appUserCreateDto.Password != appUserCreateDto.ConfirmPassword)
            {
                return new ApiResponse<string>(false, "Passwords do not match.", StatusCodes.Status400BadRequest, new List<string> { "Passwords do not match." });
            }

            var userWithEmailExists = await _userManager.FindByEmailAsync(appUserCreateDto.Email);
            if (userWithEmailExists != null)
            {
                return new ApiResponse<string>(false, "User with this email already exists.", StatusCodes.Status400BadRequest, new List<string> { "User with this email already exists." });
            }
            var userWithPhoneNumberExists = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == appUserCreateDto.PhoneNumber);
            if (userWithPhoneNumberExists != null)
            {
                return new ApiResponse<string>(false, "User with this phone number already exists.", StatusCodes.Status400BadRequest, new List<string> { "User with this phone number already exists." });
            }

            var appUser = new AppUser
            {
                FirstName = appUserCreateDto.FirstName,
                LastName = appUserCreateDto.LastName,
                Email = appUserCreateDto.Email,
                UserName = appUserCreateDto.Email,
                PhoneNumber = appUserCreateDto.PhoneNumber,
                EmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString()
            };
            try
            {
                var result = await _userManager.CreateAsync(appUser, appUserCreateDto.Password);
                if (!result.Succeeded)
                {
                    return new ApiResponse<string>(false, "User unable to register.", StatusCodes.Status400BadRequest, new List<string> { "User unable to register." });
                }
                await _userManager.AddToRoleAsync(appUser, "User");
                var emailConfirmationLink = GenerateEmailConfirmationLink(appUser.Id, appUser.EmailConfirmationToken);


                var mailRequest = new MailRequest
                {
                    ToEmail = appUser.Email,
                    Subject = "Email Confirmation",
                    Body = $"Please confirm your email by clicking <a href='{emailConfirmationLink}'>here</a>."
                };
                await services.CreateWallet(appUser.Id);
                await _emailService.SendHtmlEmailAsync(mailRequest);
                return ApiResponse<string>.Success(appUserCreateDto.Email, "Registration successful. Please check your email for confirmation instructions.", StatusCodes.Status200OK);

                //return ApiResponse<string>.Success(appUserCreateDto.Email, $"{appUserCreateDto.FirstName} registered successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a user " + ex.InnerException);
                var errorList = new List<string> { ex.InnerException?.ToString() ?? ex.Message };
                return new ApiResponse<string>(false, "Error occurred while adding a user.", StatusCodes.Status500InternalServerError, errorList);
            }
        }

        public async Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.EmailConfirmationToken != token)
            {
                return new ApiResponse<string>(false, "Invalid confirmation link.", StatusCodes.Status400BadRequest);
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            await _userManager.UpdateAsync(user);

            return new ApiResponse<string>(true, "Email confirmed successfully.", StatusCodes.Status200OK);
        }
        private string GenerateEmailConfirmationLink(string userId, string token)
        {
            string confirmationLink = $"http://localhost:3000/EmailVerifiedModal?userId={userId}&token={token}";
            return confirmationLink;
        }

        public async Task<ApiResponse<string>> ResendEmailVerifyLink(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found.", StatusCodes.Status400BadRequest, new List<string> { "You can get onboard by registering on our site." });
                }
                var emailConfirmationLink = GenerateEmailConfirmationLink(user.Id, user.EmailConfirmationToken);
                var mailRequest = new MailRequest
                {
                    ToEmail = user.Email,
                    Subject = "Email Confirmation",
                    Body = $"Please confirm your email by clicking <a href='{emailConfirmationLink}'>here</a>."
                };
                await _emailService.SendHtmlEmailAsync(mailRequest);
                return ApiResponse<string>.Success(user.Email, "Email Resent successfully. Please check your email for confirmation instructions.", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending an email" + ex.InnerException);
                var errorList = new List<string> { ex.InnerException?.ToString() ?? ex.Message };
                return new ApiResponse<string>(false, "Error occurred while sending an email.", StatusCodes.Status500InternalServerError, errorList);
            }
        }

        public async Task<ApiResponse<string>> VerifyAndAuthenticateUserAsync(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
                var userId = payload.Subject;
                var userEmail = payload.Email;
                var userName = payload.Name;
                var firstName = payload.GivenName;
                var lastName = payload.FamilyName;
                var existingUser = await _userManager.FindByEmailAsync(userEmail);
                if (existingUser == null)
                {
                    var newUser = new AppUser
                    {
                        Email = userEmail,
                        UserName = userEmail,
                        FirstName = firstName,
                        LastName = lastName,
                    };
                    var result = await _userManager.CreateAsync(newUser);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(newUser, isPersistent: false);
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, newUser.Id),
                            new Claim(ClaimTypes.Name, userName)
                        };
                        var jwtToken = GetToken(claims);
                        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                        return new ApiResponse<string>(true, "User created and authenticated successfully on the server side", StatusCodes.Status200OK, token);
                    }
                    else
                    {
                        return new ApiResponse<string>(false, "User creation failed", StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    await _signInManager.SignInAsync(existingUser, isPersistent: false);
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, existingUser.Id),
                            new Claim(ClaimTypes.Name, userName)
                        };
                    var jwtToken = GetToken(claims);
                    var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                    return new ApiResponse<string>(true, "User authenticated successfully on the server side", StatusCodes.Status200OK, token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password");
                return new ApiResponse<string>(false, "Error occurred while authenticating user", StatusCodes.Status500InternalServerError);
            }
        }
    

        public async Task<ApiResponse<string>> UpdateUserInformation(string userId, IFormFile formFile)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(nameof(userId));
                ArgumentNullException.ThrowIfNull(nameof(formFile));

                var userExist = await _saviDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userExist == null)
                {
                    return new ApiResponse<string>(false, "User not found", StatusCodes.Status404NotFound);
                }

                var image = await _cloudinaryServices.UploadImage(userId, formFile);

                // Update user's image URL
                userExist.ImageUrl = image;

                var result = await _userManager.UpdateAsync(userExist);

                if (result.Succeeded)
                {
                    return new ApiResponse<string>(true, "Account Updated successfully", StatusCodes.Status200OK);
                }
                return new ApiResponse<string>(false, "Failed to update account", StatusCodes.Status400BadRequest);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Argument is null");
                return new ApiResponse<string>(false, "Argument is null", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user");
                return new ApiResponse<string>(false, "Error occurred while updating user", StatusCodes.Status500InternalServerError);
            }
        }

    }
}

    

   
