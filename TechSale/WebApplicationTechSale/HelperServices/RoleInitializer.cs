﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplicationTechSale.HelperServices
{
    public class RoleInitializer
    {
        public static async Task InitializeRolesAsync(RoleManager<IdentityRole> roleManager, 
            List<string> roles)
        {
            foreach (var role in roles)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
