using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Services.Models
{
    public class RecipeAddModel
    {
        public RecipeAddModel()
        {
            this.Measurements = new List<string>();
        }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Preparation { get; set; }

        [Required]
        public string Course { get; set; }

        [Required]
        public string Category { get; set; }

        public int Servings { get; set; }

        public int PrepTime { get; set; }

        public int CookingTime { get; set; }

        public IFormFile Image { get; set; }

        [Required]
        public List<AddIngredient> Ingredients { get; set; }


        public List<SelectListItem> Categories { get; set; }

        public List<SelectListItem> Courses { get; set; }

        public List<string> Measurements { get; set; }
    }
}
