using DataAccessLogic.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace WebApplicationTechSale.HelperServices
{
    public class AdminInitializer
    {
        public static async Task InitializeAdmin(UserManager<User> userManager, string email, string password)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                User admin = new User { Email = email, UserName = email };
                var registerResult = await userManager.CreateAsync(admin, password);
                if (registerResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
