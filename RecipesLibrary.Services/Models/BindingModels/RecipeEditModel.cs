using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Services.Models
{
    public class RecipeEditModel
    {
        public RecipeEditModel()
        {
            this.AddedIngredients = new List<IngredientEditModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Preparation { get; set; }

        public string Course { get; set; }

        public string Category { get; set; }

        public int Servings { get; set; }

        public int PrepTime { get; set; }

        public int CookingTime { get; set; }

        public string ImageUrl { get; set; }

        public List<IngredientEditModel> Ingredients { get; set; }

        public List<IngredientEditModel> AddedIngredients { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public List<SelectListItem> Courses { get; set; }

        public List<SelectListItem> Measurements { get; set; }
    }
}
