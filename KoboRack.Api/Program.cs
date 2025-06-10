using Hangfire;
using KoboRack.Api.AutoMapperProfile;
using KoboRack.Api.Configurations;
using KoboRack.Api.Extensions;
using KoboRack.Core.IServices;
using KoboRack.Core.Services;
using KoboRack.Utility;
using Microsoft.OpenApi.Models;

namespace KoboRack.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = builder.Configuration;
            builder.Services.AddLoggingConfiguration(configuration);
            
            builder.Services.AddControllers();
            builder.Services.AddMailService(configuration);
            builder.Services.AddDependencies(configuration);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "KoboRack API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            }
                        },
                        new string[] { }
                    }
                });
            });

            //         builder.Services.AddDbContext<SaviDbContext>(options =>
            //options.UseSqlServer(configuration.GetConnectionString("SaviSavings")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

			builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddAutoMapper(typeof(MapperProfile));

            builder.Services.AddAuthenticationConfiguration(configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI( c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KoboRack v1"));
            }
            //using (var scope = app.Services.CreateScope())
            //{
            //    var serviceprovider = scope.ServiceProvider;
            //    Seeder.SeedRolesAndAdminUser(serviceprovider);
            //}
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                Seeder.SeedRolesAndAdminUser(scope.ServiceProvider).Wait();
            }

            app.UseCors("AllowAllOrigins");
            //app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<IAutoSaveBackgroundService>(
                "auto-save-task",
                x => x.CheckAndExecuteAutoSaveTask(),
                "0 10 * * *");
            RecurringJob.AddOrUpdate<IFundingAnalyticsBackgroundServices>(
                "swc-funding-analytics",
                x => x.SWCFunding(),
                "0 2 * * *");
            RecurringJob.AddOrUpdate<IAutoGroupFundingBackgroundService>(
                "auto-group-save",
                x => x.AutoGroup(),
                "0 09 * * *");
            app.MapControllers();

            app.Run();
        }
    }
}
