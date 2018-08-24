using RecipesLibrary.Data;
using RecipesLibrary.Models;
using RecipesLibrary.Services.Contracts;
using RecipesLibrary.Services.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecipesLibrary.Services.Admin
{
    public class IngredientsService:IIngredientsService
    {
        private readonly RecipesDbContext dbContext;

        public IngredientsService(RecipesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<IngredientModel> All()
        {
            var ingredients = this.dbContext
                .Ingredients
                .Select(i => new IngredientModel
                {
                    Name = i.Name,
                    Id = i.Id
                }).ToList();

            return ingredients;
        }

        public void Delete(int id)
        {
            var i = this.dbContext.Ingredients.Find(id);
            this.dbContext.Remove(i);
            this.dbContext.SaveChanges();
        }

        public void Add(string name)
        {
            var i = new Ingredient
            {
                Name = name
            };

            this.dbContext.Add(i);
            this.dbContext.SaveChanges();
        }

        public IngredientModel GetById(int id)
        {
            var ingr = this.dbContext
                .Ingredients
                .Find(id);

            var result = new IngredientModel
            {
                Name = ingr.Name,
                Id = id
            };

            return result;
        }

        public void Edit(IngredientModel model)
        {
            var ingr = this.dbContext
                .Ingredients
                .Find(model.Id);

            ingr.Name = model.Name;

            this.dbContext.SaveChanges();
        }

    }
}
