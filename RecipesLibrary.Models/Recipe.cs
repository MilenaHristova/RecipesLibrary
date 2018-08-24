using System;
using System.Collections.Generic;

namespace RecipesLibrary.Models
{
    public class Recipe
    {
        public Recipe()
        {
            this.Ingredients = new List<RecipeIngredient>();
        }

        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public DateTime AddedOn { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Preparation { get; set; }

        public int Servings { get; set; }

        public int PrepTime { get; set; }

        public int CookingTime { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<RecipeIngredient> Ingredients { get; set; }
    }
}
