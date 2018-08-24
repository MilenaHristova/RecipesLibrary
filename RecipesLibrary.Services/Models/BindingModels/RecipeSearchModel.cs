using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Services.Models
{
    public class RecipeSearchModel
    {
        public string Category { get; set; }

        public string Course { get; set; }

        public List<string> Ingredients { get; set; }
    }
}
