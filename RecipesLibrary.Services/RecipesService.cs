namespace RecipesLibrary.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using RecipesLibrary.Data;
    using RecipesLibrary.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RecipesService:IRecipesService
    {
        private readonly RecipesDbContext dbContext;

        public RecipesService(RecipesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<RecipeViewModel> All()
        {
            var recipes = this.dbContext
                .Recipes
                .OrderByDescending(r => r.AddedOn)
                .ProjectTo<RecipeViewModel>()
                .ToList();

            return recipes;
        }

        public List<RecipeViewModel> Last(int count)
        {
            var recipes = this.dbContext
                .Recipes
                .OrderByDescending(r => r.AddedOn)
                .ProjectTo<RecipeViewModel>()
                .Take(count)
                .ToList();

            return recipes;
        }

        public List<RecipeWithIngredients> AllRecipesWithIngredients()
        {
            var res = this.dbContext
                .Recipes
                .ProjectTo<RecipeWithIngredients>()
                .ToList();

            return res;
        }

        public async Task AddRecipe(RecipeAddModel model, string currentUserName)
        {
            var filePath = "";

            if(model.Image != null)
            {
                filePath = await this.UploadFile(model.Image);
            }
            else
            {
                filePath = "noimage.png";
            }
    
            var user = this.dbContext
                .Users
                .FirstOrDefault(u => u.UserName == currentUserName);

            var userId = user.Id;

            if(user == null)
            {
                throw new ArgumentException("User not found");
            }

            var course = this.dbContext
                .Courses
                .FirstOrDefault(c => c.Name == model.Course);

            if(course == null)
            {
                throw new ArgumentNullException(nameof(model.Course));
            }

            var cat = this.dbContext
                .Categories
                .FirstOrDefault(c => c.Name == model.Category);

            if (cat == null)
            {
                throw new ArgumentNullException(nameof(model.Category));
            }

            var recipe = new Recipe
            {
                Name = model.Name,
                Description = model.Description,
                Preparation = model.Preparation,
                Servings = model.Servings,
                PrepTime = model.PrepTime,
                CookingTime = model.CookingTime,
                ImageUrl = filePath,
                AuthorId = userId,
                Author = user,
                AddedOn = DateTime.Now,
                CourseId = course.Id,
                CategoryId = cat.Id,
                Course = course,
                Category = cat
            };

            var ingredients = new List<RecipeIngredient>();

            foreach (var ingr in model.Ingredients)
            {
                if(!this.dbContext.Measurements
                    .Any(m => m.Name == ingr.Measurement))
                {
                    await this.dbContext.Measurements
                        .AddAsync(new Measurement
                        {
                            Name = ingr.Measurement
                        });

                    await this.dbContext.SaveChangesAsync();
                }

                var measurement = this.dbContext
                    .Measurements
                    .FirstOrDefault(m => m.Name == ingr.Measurement);

                var quantity = ingr.Quantity;

                if(!this.dbContext.Ingredients.Any(i => i.Name == ingr.Name))
                {
                    await this.dbContext.Ingredients.AddAsync(new Ingredient
                    {
                        Name = ingr.Name
                    });

                    await this.dbContext.SaveChangesAsync();
                }

                var ingredient = this.dbContext
                    .Ingredients
                    .FirstOrDefault(i => i.Name == ingr.Name);

                ingredients.Add(new RecipeIngredient
                {
                    Measurement = measurement,
                    Quantity = quantity,
                    IngredientId = ingredient.Id
                });
            }

            recipe.Ingredients = ingredients;
            this.dbContext.Add(recipe);
            this.dbContext.SaveChanges(); 
        }

        public List<RecipeViewModel> GetByUserId(string userId)
        {
            var res = this.dbContext
                .Recipes
                .Where(r => r.AuthorId == userId)
                .ProjectTo<RecipeViewModel>()
                .ToList();

            return res;
        }

        public RecipeDetailsModel GetById(int id)
        {
            var res = this.dbContext
                .Recipes
                .Where(r => r.Id == id)
                .ProjectTo<RecipeDetailsModel>()
                .FirstOrDefault();

            return res;
        }

        public void Delete(int id)
        {
            var r = this.dbContext
                .Recipes
                .Find(id);

            this.dbContext
                .Recipes
                .Remove(r);

            this.dbContext.SaveChanges();
        }

        public void Edit(RecipeEditModel model)
        {
            var course = this.dbContext
                .Courses
                .FirstOrDefault(c => c.Name == model.Course);

            var cat = this.dbContext
                .Categories
                .FirstOrDefault(c => c.Name == model.Category);

            var recipe = this.dbContext
                .Recipes
                .Find(model.Id);

            recipe.Name = model.Name;
            recipe.Description = model.Description;
            recipe.Preparation = model.Preparation;
            recipe.Servings = model.Servings;
            recipe.PrepTime = model.PrepTime;
            recipe.CookingTime = model.CookingTime;
            recipe.ImageUrl = model.ImageUrl;
            recipe.Course = course;
            recipe.Category = cat;

            var ingredients = new List<RecipeIngredient>();

            if(model.Ingredients != null)
            {
                foreach (var ingr in model.Ingredients)
                {
                    var isAlreadyAdded = this.dbContext.RecipesIngredients
                        .Any(i => i.Ingredient.Name == ingr.Name && 
                        i.Recipe.Id == recipe.Id);

                    if (!ingr.IsDeleted && !isAlreadyAdded)
                    {
                        this.AddIngredient(ingr, ingredients, recipe);
                    }
                    else if(ingr.IsDeleted)
                    {
                        var removed = dbContext
                            .RecipesIngredients
                            .FirstOrDefault(r => r.Ingredient.Name == ingr.Name &&
                            r.RecipeId == model.Id);

                        this.dbContext.RecipesIngredients.Remove(removed);
                        this.dbContext.SaveChanges();
                    }
                }
            }
            
            foreach(var ingr in model.AddedIngredients)
            {
                if (!ingr.IsDeleted)
                {
                    this.AddIngredient(ingr, ingredients, recipe);
                }
            }


            this.dbContext.RecipesIngredients.AddRange(ingredients);
            //recipe.Ingredients = ingredients;
            this.dbContext.SaveChanges();
        }

        public RecipeEditModel GetForEdit(int id)
        {
            var allCourses = this.dbContext
                .Courses
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToList();

            var allCategories = this.dbContext
                .Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToList();

            var allMeasurements = this.dbContext
                .Measurements
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToList();

            var res = this.dbContext
                .Recipes
                .Where(r => r.Id == id)
                .ProjectTo<RecipeEditModel>()
                .FirstOrDefault();

            res.Categories = allCategories;
            res.Courses = allCourses;
            res.Measurements = allMeasurements;

            return res;
        }

        public List<RecipeViewModel> GetByCategory(string category)
        {
            var res = this.dbContext
                .Recipes
                .Where(r => r.Category.Name == category)
                .ProjectTo<RecipeViewModel>()
                .ToList();

            return res;
        }

        public bool IsSavedByUser(string username, int recipeId)
        {
            var saved = this.dbContext
                .UsersRecipes
                .FirstOrDefault(u => u.RecipeId == recipeId
                && u.User.UserName == username);

            if(saved == null)
            {
                return false;
            }

            return true;
        }

        public List<RecipeViewModel> Search(string searchTerm)
        {
            var recipes = this.dbContext
                .Recipes
                .Where(r => r.Name.ToLower().Contains(searchTerm.ToLower()))
                .ProjectTo<RecipeViewModel>()
                .ToList();

            return recipes;
        }

        private void AddIngredient(IngredientEditModel ingr,
            List<RecipeIngredient> ingredients,
            Recipe recipe)
        {
            if(!this.dbContext
                .Measurements
                .Any(m => m.Name == ingr.Measurement))
            {
                this.dbContext
                    .Measurements
                    .Add(new Measurement
                    {
                        Name = ingr.Measurement
                    });

                this.dbContext.SaveChanges();
            }

            var measurement = this.dbContext
                    .Measurements
                    .FirstOrDefault(m => m.Name == ingr.Measurement);

            var quantity = ingr.Quantity;

            if (!this.dbContext.Ingredients.Any(i => i.Name == ingr.Name))
            {
                this.dbContext.Ingredients.Add(new Ingredient
                {
                    Name = ingr.Name
                });

                this.dbContext.SaveChanges();
            }

            var ingredient = this.dbContext
                .Ingredients
                .FirstOrDefault(i => i.Name == ingr.Name);

            ingredients.Add(new RecipeIngredient
            {
                Measurement = measurement,
                Quantity = quantity,
                Ingredient = ingredient,
                Recipe = recipe
            });
        
        }

        private async Task<string> UploadFile(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot\\uploads", fileName);

            using(var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
