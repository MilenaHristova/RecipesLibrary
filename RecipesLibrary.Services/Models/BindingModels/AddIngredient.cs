using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Services.Models
{
    public class AddIngredient
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Measurement { get; set; }

        [Required]
        public double Quantity { get; set; }
    }
}
