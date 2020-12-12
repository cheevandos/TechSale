using DataAccessLogic.DatabaseModels;
using DataAccessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLogic.CrudLogic
{
    public class UserLogic : ICrudLogic<User>
    {
        private readonly ApplicationContext context;

        public UserLogic(ApplicationContext context)
        {
            this.context = context;
        }

        public Task Create(User model)
        {
            throw new NotImplementedException();
        }

        public Task Delete(User model)
        {
            throw new NotImplementedException();
        }

        public List<User> Read(User model)
        {
            return context.Users.Where(user => (model == null)
            || (!string.IsNullOrWhiteSpace(model.UserName) && user.UserName == model.UserName))
            .ToList();
        }

        public async Task Update(User model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.TelegramUsername)
                || string.IsNullOrWhiteSpace(model.TelegramChatId))
            {
                throw new Exception("Пользователь не определен");
            }

            User existingUser = await context.Users.FirstOrDefaultAsync(user =>
            user.TelegramUsername == model.TelegramUsername);
            if (existingUser == null)
            {
                throw new Exception("Пользователь с таким Telegram Username не найден в системе");
            }

            existingUser.TelegramChatId = model.TelegramChatId;
            context.Users.Update(existingUser);
            await context.SaveChangesAsync();
        }
    }
}
