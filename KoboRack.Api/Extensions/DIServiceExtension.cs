using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using KoboRack.Core.IServices;
using KoboRack.Core.Services;
using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Implementation;
using KoboRack.Data.Repositories.Interface;
using KoboRack.Data.UnitOfWork;
using KoboRack.Model.Entities;
using KoboRack.Api.AutoMapperProfile;
using Hangfire;

namespace KoboRack.Api.Extensions
{
    public static class DIServiceExtension
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration config)
        {
            var emailSettings = new EmailSettings();
            config.GetSection("EmailSettings").Bind(emailSettings);
            services.AddSingleton(emailSettings);
            services.AddScoped<IEmailServices, EmailServices>();
            var cloudinarySettings = new CloudinarySettings();
            config.GetSection("CloudinarySettings").Bind(cloudinarySettings);
            services.AddSingleton(cloudinarySettings);
            services.AddScoped(typeof(ICloudinaryServices<>), typeof(CloudinaryServices<>));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IKycService, KycService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFundingService, FundingService>();
            services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<SaviDbContext>()
            .AddDefaultTokenProviders();
            services.AddScoped<IAdminService, AdminService>();
            services.AddDbContext<SaviDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddScoped<IPersonalSavings, PersonalSavings>();
            services.AddScoped<IGroupSavings, GroupSavings>();
            services.AddScoped<ISavingRepository, SavingRepository>();
            services.AddScoped<IWalletServices, WalletServices>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/api/Authentication/Login"; 
            })
            .AddGoogle(options =>
            {
                options.ClientId = config["Google:ClientId"];
                options.ClientSecret = config["Google:ClientSecret"];
                options.CallbackPath = "/api/Authentication/signin-google/token"; 
            });
            //services.AddTransient<WalletServices>(provider =>
            //{
            //    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
            //    return new WalletServices(configuration);
            //});
            services.AddTransient<WalletServices>();
            services.AddHangfire(confi => confi.UseSqlServerStorage(config.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            services.AddScoped<IAutoSaveBackgroundService,AutoSaveBackgroundService>();
            services.AddScoped<IFundingAnalyticsBackgroundServices, FundingAnalyticsBackgroundServices>();
            services.AddScoped<IAutoGroupFundingBackgroundService, AutoGroupFundingBackgroundService>();
            services.AddScoped<IGroupSavingsMembersServices, GroupSavingsMembersServices>();
            services.AddScoped<IGroupSavingsMembersRepository, GroupSavingsMembersRepository>();

        }
    }
}