using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RecipesLibrary.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.Recipes = new List<Recipe>();
        }

        public ICollection<Recipe> Recipes { get; set; }
    }
}
