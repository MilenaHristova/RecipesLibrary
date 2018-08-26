using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RecipesLibrary.Services;
using RecipesLibrary.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecipesLibrary.Services.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecipesLibrary.Infrastructure;
using RecipesLibrary.Infrastructure.Extensions;
using RecipesLibrary.Models;

namespace RecipesLibrary.Controllers
{
    public class UsersController:Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var user = this.usersService.GetByUsername(this.User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Recipes()
        {
            var user = this.User.Identity.Name;

            var recipes = this.usersService
                .GetRecipesByUserName(user);

            if (recipes == null)
            {
                return NotFound();
            }

            return View(recipes);
        }

        [HttpGet]
        public IActionResult Saved()
        {
            var user = this.User.Identity.Name;

            var recipes = this.usersService
                .GetSavedByUserName(user);

            if(recipes == null)
            {
                return NotFound();
            }

            return View(recipes);
        }

        [HttpGet]
        public IActionResult Save(int id)
        {
            var username = this.User.Identity.Name;

            try
            {
                this.usersService.Save(username, id);
                this.AddAlertSuccess("Recipe is saved");
            }
            catch(Exception)
            {
                this.AddAlertDanger("Sorry, something went wrong.");
            }

            return this.Redirect($"/Recipes/Details/{id}");
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            var username = this.User.Identity.Name;

            try
            {
                this.usersService.Remove(username, id);
                this.AddAlertSuccess("Recipe is removed");
            }
            catch (Exception)
            {
                this.AddAlertDanger("Sorry, something went wrong.");
            }

            return this.Redirect($"/Recipes/Details/{id}");
        }
        
    }
}
