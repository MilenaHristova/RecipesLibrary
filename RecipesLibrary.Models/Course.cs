using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Models
{
    public class Course
    {
        public Course()
        {
            this.Recipes = new List<Recipe>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Recipe> Recipes { get; set; }
    }
}
