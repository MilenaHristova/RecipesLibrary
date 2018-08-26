using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesLibrary.Infrastructure;
using RecipesLibrary.Services.Contracts;

namespace RecipesLibrary.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = GlobalConstants.AdministratorRole)]
    public class CoursesController:Controller
    {
        private readonly ICoursesService coursesService;

        public CoursesController(ICoursesService coursesService)
        {
            this.coursesService = coursesService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string course)
        {
            this.coursesService.Add(course);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public IActionResult All()
        {
            var all = this.coursesService
                .AllWithIds();

            return View(all);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            this.coursesService.Delete(id);

            return RedirectToAction(nameof(All));
        }
    }
}
