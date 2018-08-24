using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Services.Models
{
    public class UserWithRecipesModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public List<RecipeViewModel> Recipes { get; set; }
    }
}
