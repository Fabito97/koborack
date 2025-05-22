using Hangfire;
using KoboRack.Api.AutoMapperProfile;
using KoboRack.Api.Configurations;
using KoboRack.Api.Extensions;
using KoboRack.Core.IServices;
using KoboRack.Core.Services;
using KoboRack.Utility;

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
            builder.Services.AddSwaggerGen();

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
			builder.Services.AddSwaggerGen();
			builder.Services.AddCors();

            builder.Services.AddAutoMapper(typeof(MapperProfile));



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI( c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Savi v1"));
            }
            //using (var scope = app.Services.CreateScope())
            //{
            //    var serviceprovider = scope.ServiceProvider;
            //    Seeder.SeedRolesAndAdminUser(serviceprovider);
            //}
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //Seeder.SeedRolesAndAdminUser(scope.ServiceProvider).Wait(); 
            }

            app.UseCors(p => p.AllowAnyOrigin()
                .AllowAnyHeader().AllowAnyMethod());
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("AllowAllOrigins");

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
