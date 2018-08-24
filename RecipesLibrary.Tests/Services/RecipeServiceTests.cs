using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RecipesLibrary.Data;
using RecipesLibrary.Models;
using RecipesLibrary.Services;
using RecipesLibrary.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RecipesLibrary.Tests
{
    public class RecipeServiceTests
    {
        [Fact]
        public async Task AddRecipeWithEmptyCourseShouldThrowException()
        {
            var db = this.GetDatabase();

            var recipesService = new RecipesService(db);

            db.Categories
                .Add(new Category
                {
                    Name = "testCategory"
                });

            db.Users
                .Add(new User
                {
                    UserName = "test",
                    Email = "test",
                    PasswordHash = "test",
                    Id = "testUserId"
                });

            db.SaveChanges();

            var model = new RecipeAddModel
            {
                Name = "testRecipe",
                Category = "testCategory",
                Ingredients = new List<AddIngredient>()
            };

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await recipesService.AddRecipe(model, "test"));
        }

        [Fact]
        public async Task AddRecipeWithEmptyCategoryShouldThrowException()
        {
            var db = this.GetDatabase();

            var recipesService = new RecipesService(db);

            db.Courses
                .Add(new Course
                {
                    Name = "testCourse"
                });

            db.Users
                .Add(new User
                {
                    UserName = "test",
                    Email = "test",
                    PasswordHash = "test",
                    Id = "testUserId"
                });

            db.SaveChanges();

            var model = new RecipeAddModel
            {
                Name = "testRecipe",
                Course = "testCourse",
                Ingredients = new List<AddIngredient>()
            };

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await recipesService.AddRecipe(model, "test"));
        }

        [Fact]
        public async Task AddRecipeShouldAddRecipeToDatabase()
        {
            var db = this.GetDatabase();

            var recipesService = new RecipesService(db);

            db.Courses
                .Add(new Course
                {
                    Name = "testCourse"
                });

            db.Categories
                .Add(new Category
                {
                    Name = "testCategory"
                });

            db.Users
                .Add(new User
                {
                    UserName = "test",
                    Email = "test",
                    PasswordHash = "test",
                    Id = "testUserId"
                });

            db.SaveChanges();

            var model = new RecipeAddModel
            {
                Name = "testRecipe",
                Course = "testCourse",
                Category = "testCategory",
                Ingredients = new List<AddIngredient>()
            };

            await recipesService.AddRecipe(model, "test");

            var added = db.Recipes.Where(r => r.Name == "testRecipe");

            added.Should().NotBeNull();
        }

        [Fact]
        public async Task AddRecipeWithExistingIngredientShouldNotAddItToDatabase()
        {
            var db = this.GetDatabase();

            var recipesService = new RecipesService(db);

            db.Courses
                .Add(new Course
                {
                    Name = "testCourse"
                });

            db.Categories
                .Add(new Category
                {
                    Name = "testCategory"
                });

            db.Ingredients
                .Add(new Ingredient
                {
                    Name = "testIngredient"
                });

            db.Users
                .Add(new User
                {
                    UserName = "test",
                    Email = "test",
                    PasswordHash = "test",
                    Id = "testUserId"
                });

            db.SaveChanges();

            var model = new RecipeAddModel
            {
                Name = "testRecipe",
                Course = "testCourse",
                Category = "testCategory",
                Ingredients = new List<AddIngredient>
                {
                    new AddIngredient
                    {
                        Name = "testIngredient"
                    }
                }
            };

            await recipesService.AddRecipe(model, "test");

            var ingr = db.Ingredients.Where(r => r.Name == "testIngredient");

            ingr.Count().Should().Be(1);
        }

        [Fact]
        public async Task AddRecipeWithExistingIngredientShouldAddItToRecipe()
        {
            var db = this.GetDatabase();

            var recipesService = new RecipesService(db);

            db.Courses
                .Add(new Course
                {
                    Name = "testCourse"
                });

            db.Categories
                .Add(new Category
                {
                    Name = "testCategory"
                });

            db.Ingredients
                .Add(new Ingredient
                {
                    Name = "testIngredient"
                });

            db.Users
                .Add(new User
                {
                    UserName = "test",
                    Email = "test",
                    PasswordHash = "test",
                    Id = "testUserId"
                });

            db.SaveChanges();

            var model = new RecipeAddModel
            {
                Name = "testRecipe",
                Course = "testCourse",
                Category = "testCategory",
                Ingredients = new List<AddIngredient>
                {
                    new AddIngredient
                    {
                        Name = "testIngr"
                    }
                }
            };

            await recipesService.AddRecipe(model, "test");

            var ingr = db.Recipes
                .FirstOrDefault(r => r.Name == "testRecipe")
                .Ingredients
                .FirstOrDefault(i => i.Ingredient.Name == "testIngr");

            ingr.Should().NotBeNull();
        }

        [Fact]
        public async Task AddRecipeWithNewIngredientShouldAddItToDatabase()
        {
            var db = this.GetDatabase();

            var recipesService = new RecipesService(db);

            db.Courses
                .Add(new Course
                {
                    Name = "testCourse"
                });

            db.Categories
                .Add(new Category
                {
                    Name = "testCategory"
                });

            db.Users
                .Add(new User
                {
                    UserName = "test",
                    Email = "test",
                    PasswordHash = "test",
                    Id = "testUserId"
                });

            db.SaveChanges();

            var model = new RecipeAddModel
            {
                Name = "testRecipe",
                Course = "testCourse",
                Category = "testCategory",
                Ingredients = new List<AddIngredient>
                {
                    new AddIngredient
                    {
                        Name = "testIngr"
                    }
                }
            };

            await recipesService.AddRecipe(model, "test");

            var ingr = db.Ingredients.Where(r => r.Name == "testIngr");

            ingr.Count().Should().Be(1);
        }

        [Fact]
        public async Task AddRecipeShouldAddRecipeToUserRecipes()
        {
            var db = this.GetDatabase();

            var recipesService = new RecipesService(db);

            db.Courses
                .Add(new Course
                {
                    Name = "testCourse"
                });

            db.Categories
                .Add(new Category
                {
                    Name = "testCategory"
                });

            db.Ingredients
                .Add(new Ingredient
                {
                    Name = "testIngredient"
                });

            db.Users
                .Add(new User
                {
                    UserName = "test",
                    Email = "test",
                    PasswordHash = "test",
                    Id = "testUserId"
                });

            db.SaveChanges();

            var model = new RecipeAddModel
            {
                Name = "testRecipe",
                Course = "testCourse",
                Category = "testCategory",
                Ingredients = new List<AddIngredient>()
            };

            await recipesService.AddRecipe(model, "test");

            var userRecipes = db.Users.Find("testUserId").Recipes;

            userRecipes.Count().Should().Be(1);
        }

        [Fact]


        private RecipesDbContext GetDatabase()
        {
            var dbOptions = new DbContextOptionsBuilder<RecipesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new RecipesDbContext(dbOptions);

            return db;
        }

    }
}
