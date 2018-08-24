using System.Collections.Generic;

namespace RecipesLibrary.Models
{
    public class Ingredient
    {
        public Ingredient()
        {
            this.Recipes = new List<RecipeIngredient>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<RecipeIngredient> Recipes { get; set; }
    }
}
