using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Services.Models
{
    public class RecipeDetailsModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public string Course { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public string AddedOn { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Preparation { get; set; }

        public int Servings { get; set; }

        public int PrepTime { get; set; }

        public int CookingTime { get; set; }

        public List<IngredientModel> Ingredients { get; set; }
    }
}

