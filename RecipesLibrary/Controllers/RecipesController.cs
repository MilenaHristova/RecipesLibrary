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

namespace RecipesLibrary.Controllers
{
    public class RecipesController:Controller
    {
        private readonly IRecipesService recipesService;
        private readonly ICategoriesService categoriesService;
        private readonly ICoursesService coursesService;
        private readonly IMeasurementsService measurementsService;

        public RecipesController(IRecipesService recipesService,
            ICategoriesService categoriesService,
            ICoursesService coursesService,
            IMeasurementsService measurementsService)
        {
            this.recipesService = recipesService;
            this.categoriesService = categoriesService;
            this.coursesService = coursesService;
            this.measurementsService = measurementsService;
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

            var res = ordered.Select(o => o.Key).ToList();

            return PartialView("SearchResult", res);
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
        public IActionResult SearchResult(List<RecipeWithIngredients> model)
        {
            var values = this.RouteData.Values["model"];
            return View(values);
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

            return this.Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
                var res = this.recipesService
                .GetById(id);

                return View(res);
            
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete(int Id)
        {
            this.recipesService
                .Delete(Id);

            return Redirect("/Home/Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var recipe = this.recipesService
                 .GetForEdit(id);

            return View(recipe);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(RecipeEditModel model)
        {
            this.recipesService.Edit(model);

            return Redirect("/Recipes/Details/" + model.Id);
        }
    }
}
