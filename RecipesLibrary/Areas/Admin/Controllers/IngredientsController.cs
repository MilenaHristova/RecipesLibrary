using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesLibrary.Infrastructure;
using RecipesLibrary.Services.Contracts;
using RecipesLibrary.Services.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = GlobalConstants.AdministratorRole)]
    public class IngredientsController:Controller
    {
        private readonly IIngredientsService service;

        public IngredientsController(IIngredientsService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult All()
        {
            var ingr = this.service.All();
            return View(ingr);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            this.service.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string ingr)
        {
            this.service.Add(ingr);
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ingr = this.service.GetById(id);
            return View(ingr);
        }

        [HttpPost]
        public IActionResult Edit(IngredientModel ingr)
        {
            this.service.Edit(ingr);
            return RedirectToAction("All");
        }
    }
}
