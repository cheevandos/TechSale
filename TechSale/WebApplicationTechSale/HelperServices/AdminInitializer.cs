﻿using DataAccessLogic.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace WebApplicationTechSale.HelperServices
{
    public class AdminInitializer
    {
        public static async Task InitializeAdmin(UserManager<User> userManager, IConfiguration configuration)
        {
            string email = configuration["AdminEmail"];
            string password = configuration["AdminPassword"];
            string username = configuration["AdminUsername"];
            if (await userManager.FindByEmailAsync(email) == null)
            {
                User admin = new User 
                { 

                    Email = email, 
                    UserName = username 
                };
                var registerResult = await userManager.CreateAsync(admin, password);
                if (registerResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
