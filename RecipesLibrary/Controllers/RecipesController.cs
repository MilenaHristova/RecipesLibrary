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
    public class RecipesController:Controller
    {
        private readonly IRecipesService recipesService;
        private readonly ICategoriesService categoriesService;
        private readonly ICoursesService coursesService;
        private readonly IMeasurementsService measurementsService;
        private readonly UserManager<User> userManager;

        public RecipesController(IRecipesService recipesService,
            ICategoriesService categoriesService,
            ICoursesService coursesService,
            IMeasurementsService measurementsService,
            UserManager<User> userManager)
        {
            this.recipesService = recipesService;
            this.categoriesService = categoriesService;
            this.coursesService = coursesService;
            this.measurementsService = measurementsService;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult All(int? page)
        {
            var recipes = this.recipesService.All();

            int pageSize = 6;
            var paginatedRecipes = PaginatedList<RecipeViewModel>
                .CreateAsync(recipes.AsQueryable(), page ?? 1, pageSize);

            return View(paginatedRecipes);
        }

        [HttpGet]
        public IActionResult SearchByIngredients()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchByIngredients(RecipeSearchModel model)
        {
            var allRecipes = this.recipesService
                .AllRecipesWithIngredients();

            var ingredients = model.Ingredients;

            var rating = new Dictionary<RecipeWithIngredients, int>();

            foreach (var rec in allRecipes)
            {
                rating.Add(rec, 0);
                foreach (var ingr in ingredients)
                {
                    if (rec.Ingredients.Contains(ingr))
                    {
                        rating[rec]++;
                    }
                }
            }

            var ordered = rating
                .Where(o => o.Value > 0)
                .OrderByDescending(r => r.Value);

            var recipes = ordered.Select(o => o.Key).ToList();

            return PartialView("SearchResult", recipes);
        }

        [HttpGet]
        public IActionResult SearchByCategory(string category)
        {
            if (!this.categoriesService.ExistsByName(category))
            {
                return NotFound();
            }

            var recipes = this.recipesService
                .GetByCategory(category);

            return View("Search", recipes);
        }

        [HttpGet]
        public IActionResult Search(string searchTerm)
        {
            var recipes = this.recipesService
                .Search(searchTerm);

            return View("Search", recipes);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            var allCourses = this.coursesService
                .All()
                .Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();

            var allCategories = this.categoriesService
                .All()
                .Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();

            var allMeasurements = this.measurementsService
                .All();

            var model = new RecipeAddModel
            {
                Categories = allCategories,
                Courses = allCourses,
                Measurements = allMeasurements
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(RecipeAddModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach(var modelState in ModelState.Values)
                {
                    foreach(var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                this.TempData["error"] = errors;
                return RedirectToAction("Add", model);
            }

            var currentUser = this.User.Identity.Name;

            await this.recipesService.AddRecipe(model, currentUser);

            this.AddAlertSuccess("Successfully added recipe");
            return this.Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var res = this.recipesService
                .GetById(id);

            if (User.Identity.IsAuthenticated)
            {
                var user = this.User.Identity.Name;
                if (this.recipesService.IsSavedByUser(user, id))
                {
                    res.IsSavedByCurrentUser = true;
                }
                else
                {
                    res.IsSavedByCurrentUser = false;
                }
            }
            
            if(res == null)
            {
                return NotFound();
            }

            return View(res);  
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete(int Id)
        {
            var username = this.User.Identity.Name;
            var recipe = this.recipesService.GetById(Id);
            if(recipe == null || recipe.Author != username)
            {
                return NotFound();
            }

            try
            {
                this.recipesService
                .Delete(Id);

                this.AddAlertInfo("Recipe is deleted.");
            }
            catch (Exception)
            {
                this.AddAlertDanger("Sorry, something went wrong.");
            }
           
            return Redirect("/Home/Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var recipe = this.recipesService
                 .GetForEdit(id);

            if(recipe == null || recipe.Author != this.User.Identity.Name)
            {
                return this.NotFound();
            }

            return View(recipe);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(RecipeEditModel model)
        {
            var recipe = this.recipesService
                 .GetById(model.Id);

            if (recipe == null || recipe.Author != this.User.Identity.Name)
            {
                return this.NotFound();
            }

            this.recipesService.Edit(model);

            this.AddAlertSuccess("The Recipe was successfully eddited.");
            return Redirect("/Recipes/Details/" + model.Id);
        }
    }
}
