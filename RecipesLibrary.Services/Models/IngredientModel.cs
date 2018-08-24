using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Services.Models
{
    public class IngredientModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Measurement { get; set; }

        public double Quantity { get; set; }
    }
}
