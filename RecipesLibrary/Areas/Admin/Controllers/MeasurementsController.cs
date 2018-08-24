using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesLibrary.Infrastructure;
using RecipesLibrary.Services.Contracts;

namespace RecipesLibrary.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = GlobalConstants.AdministratorRole)]
    public class MeasurementsController:Controller
    {
        private readonly IMeasurementsService measurementsService;

        public MeasurementsController(IMeasurementsService measurementsService)
        {
            this.measurementsService = measurementsService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string m)
        {
            this.measurementsService.Add(m);

            return Redirect("/Home/Index");
        }
    }
}
