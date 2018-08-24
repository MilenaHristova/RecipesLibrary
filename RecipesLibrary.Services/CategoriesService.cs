using RecipesLibrary.Data;
using RecipesLibrary.Models;
using RecipesLibrary.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecipesLibrary.Services
{
    public class CategoriesService:ICategoriesService
    {
        private readonly RecipesDbContext dbContext;

        public CategoriesService(RecipesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Add(string name)
        {
            this.dbContext
                .Add(new Category
                {
                    Name = name
                });

            this.dbContext.SaveChanges();
        }

        public List<string> All()
        {
            var res = this.dbContext
                .Categories
                .Select(c => c.Name)
                .ToList();

            return res;
        }

        public bool ExistsByName(string name)
        {
            return this.dbContext
                .Categories
                .Any(c => c.Name == name);
        }
    }
}
