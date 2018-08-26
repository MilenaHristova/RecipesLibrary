using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace RecipesLibrary.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.Recipes = new List<Recipe>();
            this.SavedRecipes = new List<UserRecipe>();
        }

        public ICollection<Recipe> Recipes { get; set; }

        public ICollection<UserRecipe> SavedRecipes { get; set; }
    }
}
