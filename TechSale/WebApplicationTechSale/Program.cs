using DataAccessLogic.DatabaseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationTechSale.HelperServices;

namespace WebApplicationTechSale
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                IServiceProvider provider = scope.ServiceProvider;
                try
                {
                    var userManager = provider.GetRequiredService<UserManager<User>>();
                    var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

                    IConfigurationBuilder configBuilder = new ConfigurationBuilder().AddJsonFile("basedata.json");
                    IConfigurationRoot configuration = configBuilder.Build();

                    IConfigurationSection section = configuration.GetSection("BaseRoles");

                    List<string> baseRoles = section.Get<List<string>>();

                    string adminEmail = configuration["AdminData:Email"];
                    string adminPassword = configuration["AdminData:Password"];

                    await RoleInitializer.InitializeRolesAsync(roleManager, baseRoles);
                    await AdminInitializer.InitializeAdmin(userManager, adminEmail, adminPassword);
                } 
                catch (Exception ex)
                {
                    var logger = provider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Произошла ошибка при работе с БД");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
