using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Services.Models
{
    public class RecipeSearchModel
    {
        public List<string> Ingredients { get; set; }
    }
}
