using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesLibrary.Infrastructure;
using RecipesLibrary.Services.Contracts;

namespace RecipesLibrary.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = GlobalConstants.AdministratorRole)]
    public class CategoriesController:Controller
    {
        private readonly ICategoriesService categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string category)
        {
            this.categoriesService.Add(category);

            return Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult All()
        {
            var all = this.categoriesService
                .AllWithIds();

            return View(all);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            this.categoriesService.Delete(id);

            return RedirectToAction(nameof(All));
        }
    }
}
