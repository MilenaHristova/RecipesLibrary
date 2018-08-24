using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Services.Models
{
    public class RecipeWithIngredients
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public List<string> Ingredients { get; set; }
    }
}
