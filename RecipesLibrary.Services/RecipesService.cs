namespace RecipesLibrary.Services
{
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
                .Select(r => new RecipeViewModel
                {
                    Name = r.Name,
                    Description = r.Description,
                    AddedOn = $"{r.AddedOn.Day}/{r.AddedOn.Month}/{r.AddedOn.Year}",
                    Author = r.Author.UserName,
                    Id = r.Id,
                    ImageUrl = r.ImageUrl
                })
                .ToList();

            recipes = recipes.OrderByDescending(r => r.AddedOn).ToList();

            return recipes;
        }

        public List<RecipeViewModel> Last(int count)
        {
            var recipes = this.dbContext
                .Recipes
                .Select(r => new RecipeViewModel
                {
                    Name = r.Name,
                    Description = r.Description,
                    AddedOn = $"{r.AddedOn.Day}/{r.AddedOn.Month}/{r.AddedOn.Year}",
                    Author = r.Author.UserName,
                    Id = r.Id,
                    ImageUrl = r.ImageUrl
                })
                .OrderByDescending(r => r.AddedOn)
                .Take(count)
                .ToList();

            return recipes;
        }

        public List<RecipeWithIngredients> AllRecipesWithIngredients()
        {
            var res = this.dbContext
                .Recipes
                .Select(r => new RecipeWithIngredients
                {
                    Name = r.Name,
                    Id = r.Id,
                    ImageUrl = r.ImageUrl,
                    Ingredients = r.Ingredients
                       .Select(i => i.Ingredient.Name).ToList()
                }).ToList();

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
                .Select(r => new RecipeViewModel
                {
                    Name = r.Name,
                    Description = r.Description,
                    Id = r.Id,
                    AddedOn = $"{r.AddedOn.Day}/{r.AddedOn.Month}/{r.AddedOn.Year}",
                    ImageUrl = r.ImageUrl
                })
                .ToList();

            return res;
        }

        public RecipeDetailsModel GetById(int id)
        {
            var res = this.dbContext
                .Recipes
                .Where(r => r.Id == id)
                .Select(recipe => new RecipeDetailsModel
                {
                    Name = recipe.Name,
                    Author = recipe.Author.UserName,
                    Description = recipe.Description,
                    AddedOn = $"{recipe.AddedOn.Day}/{recipe.AddedOn.Month}/{recipe.AddedOn.Year}",
                    ImageUrl = recipe.ImageUrl,
                    Ingredients = recipe.Ingredients.Select(i => new IngredientModel
                    {
                        Name = i.Ingredient.Name,
                        Quantity = i.Quantity,
                        Measurement = i.Measurement.Name
                    }).ToList(),
                    Category = recipe.Category.Name,
                    Course = recipe.Course.Name,
                    Preparation = recipe.Preparation,
                    PrepTime = recipe.PrepTime,
                    CookingTime = recipe.CookingTime,
                    Servings = recipe.Servings,
                    Id = recipe.Id
                }).FirstOrDefault();

            if(res == null)
            {
                throw new ArgumentException("Recipe not found");
            }

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
                    if (!ingr.IsDeleted)
                    {
                        this.AddIngredient(ingr, ingredients, recipe);
                    }
                    else
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

            recipe.Ingredients = ingredients;

            this.dbContext.Attach(recipe);
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
                .Select(recipe => new RecipeEditModel
                {
                    Categories = allCategories,
                    Courses = allCourses,
                    Measurements = allMeasurements,
                    Name = recipe.Name,
                    Description = recipe.Description,
                    ImageUrl = recipe.ImageUrl,
                    Ingredients = recipe.Ingredients.Select(i => new IngredientEditModel
                    {
                        Name = i.Ingredient.Name,
                        Quantity = i.Quantity,
                        Measurement = i.Measurement.Name
                    }).ToList(),
                    Category = recipe.Category.Name,
                    Course = recipe.Course.Name,
                    Preparation = recipe.Preparation,
                    PrepTime = recipe.PrepTime,
                    CookingTime = recipe.CookingTime,
                    Servings = recipe.Servings,
                    Id = recipe.Id
                }).FirstOrDefault();

            if (res == null)
            {
                throw new ArgumentException("Recipe not found");
            }

            return res;
        }

        public List<RecipeViewModel> GetByCategory(string category)
        {
            var res = this.dbContext
                .Recipes
                .Where(r => r.Category.Name == category)
                .Select(r => new RecipeViewModel
                {
                    Name = r.Name,
                    Description = r.Description,
                    Id = r.Id,
                    Author = r.Author.UserName,
                    AddedOn = $"{r.AddedOn.Day}/{r.AddedOn.Month}/{r.AddedOn.Year}",
                    ImageUrl = r.ImageUrl
                })
                .ToList();

            return res;
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

        public List<RecipeViewModel> Search(string searchTerm)
        {
            var recipes = this.dbContext
                .Recipes
                .Where(r => r.Name.ToLower().Contains(searchTerm.ToLower()))
                .Select(r => new RecipeViewModel
                {
                    Name = r.Name,
                    Description = r.Description,
                    Id = r.Id,
                    Author = r.Author.UserName,
                    ImageUrl = r.ImageUrl,
                    AddedOn = $"{r.AddedOn.Day}/{r.AddedOn.Month}/{r.AddedOn.Year}",
                }).ToList();

            return recipes;
        }
    }
}
