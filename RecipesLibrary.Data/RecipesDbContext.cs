using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipesLibrary.Models;

namespace RecipesLibrary.Data
{
    public class RecipesDbContext : IdentityDbContext<User>
    {
        public RecipesDbContext(DbContextOptions<RecipesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<RecipeIngredient> RecipesIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RecipeIngredient>()
                .HasOne(r => r.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(r => r.RecipeId);

            builder.Entity<RecipeIngredient>()
                .HasOne(r => r.Ingredient)
                .WithMany(i => i.Recipes)
                .HasForeignKey(r => r.IngredientId);

            builder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany(a => a.Recipes)
                .HasForeignKey(r => r.AuthorId);

            builder.Entity<Recipe>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CourseId);

            builder.Entity<Recipe>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CategoryId);

            builder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany(a => a.Recipes)
                .HasForeignKey(r => r.AuthorId);

            base.OnModelCreating(builder);
        }
    }
}
