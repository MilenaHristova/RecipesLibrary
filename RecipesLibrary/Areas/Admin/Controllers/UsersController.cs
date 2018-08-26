using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipesLibrary.Areas.Admin.Models;
using RecipesLibrary.Data;
using RecipesLibrary.Infrastructure;
using RecipesLibrary.Infrastructure.Extensions;
using RecipesLibrary.Models;
using RecipesLibrary.Services.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = GlobalConstants.AdministratorRole)]
    public class UsersController:Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> rolesManager;

        public UsersController(UserManager<User> userManager
            , RoleManager<IdentityRole> rolesManager)
        {
            this.userManager = userManager;
            this.rolesManager = rolesManager;
        }

        [HttpGet]
        public IActionResult All()
        {
            var users = this.userManager.Users
                .Select(u => new UserModel
                {
                    Username = u.UserName,
                    Id = u.Id
                })
                .ToList();

            return View(users);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email
            };

            await this.userManager.CreateAsync(user, model.Password);
            await this.userManager.AddToRoleAsync(user, GlobalConstants.UserRole);

            this.AddAlertSuccess("User Account created.");

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Roles(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if(user == null)
            {
                return NotFound();
            }

            var roles = await this.userManager.GetRolesAsync(user);

            return View(roles);     
        }

        [HttpGet]
        public IActionResult AddToRole(string id)
        {
            var roles = this.rolesManager.Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }).ToList();

            this.ViewData["id"] = id;
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(string id, string role)
        {
            var user = await this.userManager.FindByIdAsync(id);

            await this.userManager.AddToRoleAsync(user, role);

            this.AddAlertSuccess("User is added to role.");
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await this.userManager.DeleteAsync(user);
            this.AddAlertSuccess("User Account is deleted.");
            return this.RedirectToAction(nameof(All));
        }
    }
}
