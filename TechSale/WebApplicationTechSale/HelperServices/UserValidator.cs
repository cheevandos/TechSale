using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplicationTechSale.HelperServices
{
    public class UserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            User existingEmail = manager.FindByEmailAsync(user.Email).Result;
            if (existingEmail != null)
            {
                errors.Add(new IdentityError
                {
                    Description = "Данный Email уже используется"
                });
            }

            if (user.UserName.Contains(ApplicationConstantsProvider.AvoidValidationCode())) 
            {
                user.UserName = user.UserName.Replace(ApplicationConstantsProvider.AvoidValidationCode(), string.Empty);
            }
            else
            {
                User existingUserName = manager.FindByNameAsync(user.UserName).Result;
                if (existingUserName != null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Имя пользователя занято"
                    });
                }
            }

            if (user.UserName.Contains("admin") || user.UserName.Contains("moderator"))
            {
                errors.Add(new IdentityError
                {
                    Description = "Ник пользователя не должен содержать слова 'admin'"
                });
            }

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
