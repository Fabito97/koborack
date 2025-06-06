﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KoboRack.Api.Extensions
{
    public static class AuthenticationServiceExtension
    {
        public static void AddAuthenticationConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var secretKey = configuration.GetSection("JwtSettings:Secret");

            var TokenParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = configuration["JwtSettings:ValidAudience"],
                ValidIssuer = configuration["JwtSettings:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding
                    .UTF8.GetBytes(secretKey.Value)),
                ClockSkew = TimeSpan.Zero
            };
            serviceCollection.AddSingleton(TokenParameters);
            serviceCollection.AddAuthentication(options =>
            {                    
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = TokenParameters;
            });
        }
    }
}
