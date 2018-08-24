using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecipesLibrary.Models;
using RecipesLibrary.Services;
using RecipesLibrary.Services.Contracts;

namespace RecipesLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRecipesService recipesService;
        private readonly ICategoriesService categoriesService;

        public HomeController(IRecipesService recipesService,
            ICategoriesService categoriesService)
        {
            this.recipesService = recipesService;
            this.categoriesService = categoriesService;
        }

        public IActionResult Index()
        {
            var latest = this.recipesService.Last(6);
            var categories = this.categoriesService.All();

            this.ViewData["categories"] = categories;

            return View(latest);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
