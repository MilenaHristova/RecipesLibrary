using RecipesLibrary.Data;
using RecipesLibrary.Models;
using RecipesLibrary.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RecipesLibrary.Services.Models;
using AutoMapper.QueryableExtensions;

namespace RecipesLibrary.Services
{
    public class UsersService : IUsersService
    {
        private readonly RecipesDbContext dbContext;

        public UsersService(RecipesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public UserViewModel GetByUsername(string username)
        {
            var user = this.dbContext
                .Users
                .ProjectTo<UserViewModel>()
                .FirstOrDefault(u => u.Username == username);
                
            return user;
        }

        public List<RecipeViewModel> GetRecipesByUserName(string username)
        {
            var recipes = this.dbContext
                .Recipes
                .Where(r => r.Author.UserName == username)
                .ProjectTo<RecipeViewModel>()
                .ToList();

            return recipes;
        }

        public List<RecipeViewModel> GetSavedByUserName(string username)
        {
            var user = this.dbContext
                .Users
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return null;
            }

            var recipes = this.dbContext
                .UsersRecipes
                .Where(u => u.User.UserName == username)
                .Select(u => u.Recipe)
                .AsQueryable()
                .ProjectTo<RecipeViewModel>()
                .ToList();
             
            return recipes;
        }

        public void Remove(string username, int id)
        {
            var userRecipe = this.dbContext
              .UsersRecipes
              .FirstOrDefault(u => u.User.UserName == username
              && u.RecipeId == id);

            if (userRecipe == null)
            {
                throw new ArgumentNullException("Recipe not found");
            }

            this.dbContext.Remove(userRecipe);

            this.dbContext.SaveChanges();
        }

        public void Save(string username, int id)
        {
            var user = this.dbContext
                .Users
                .FirstOrDefault(u => u.UserName == username);

            if(user == null)
            {
                throw new ArgumentNullException("User not found");
            }

            var recipe = this.dbContext
                .Recipes
                .Find(id);

            if (recipe == null)
            {
                throw new ArgumentNullException("Recipe not found");
            }

            var userRecipe = new UserRecipe
            {
                User = user,
                Recipe = recipe
            };

            this.dbContext.Add(userRecipe);
            user.SavedRecipes.Add(userRecipe);
            
            this.dbContext.SaveChanges();
        }
    }
}
