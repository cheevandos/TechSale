﻿using DataAccessLogic.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> userManager;

        public AdminController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            List<User> users = new List<User>();
            foreach (User user in await userManager.Users.ToListAsync())
            {
                if (await userManager.IsInRoleAsync(user, "moderator"))
                {
                    users.Add(user);
                }
            }
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User moderator = new User { Email = model.Email, UserName = model.Email };
                var registerResult = await userManager.CreateAsync(moderator, model.Password);
                if (registerResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(moderator, "moderator");
                    return RedirectToAction("UsersList");
                }
                else
                {
                    foreach (var error in registerResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}
