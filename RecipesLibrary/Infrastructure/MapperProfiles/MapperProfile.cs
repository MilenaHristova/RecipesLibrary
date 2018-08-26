using AutoMapper;
using RecipesLibrary.Models;
using RecipesLibrary.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecipesLibrary.Infrastructure.MapperProfiles
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<Recipe, RecipeViewModel>()
                .ForMember(m => m.Author, c => c.MapFrom(r => r.Author.UserName))
                .ForMember(m => m.AddedOn, c => c.MapFrom(r => $"{r.AddedOn.Day}/{r.AddedOn.Month}/{r.AddedOn.Year}"));

            CreateMap<Recipe, RecipeWithIngredients>()
                .ForMember(m => m.Ingredients, c => c.MapFrom(r => r.Ingredients.Select(i => i.Ingredient.Name)));

            CreateMap<RecipeIngredient, IngredientViewModel>()
                .ForMember(m => m.Name, c => c.MapFrom(r => r.Ingredient.Name))
                .ForMember(m => m.Measurement, c => c.MapFrom(r => r.Measurement.Name));

            CreateMap<Recipe, RecipeDetailsModel>()
                .ForMember(m => m.Author, c => c.MapFrom(r => r.Author.UserName))
                .ForMember(m => m.Course, c => c.MapFrom(r => r.Course.Name))
                .ForMember(m => m.Category, c => c.MapFrom(r => r.Category.Name))
                .ForMember(m => m.AddedOn, c => c.MapFrom(r => $"{r.AddedOn.Day}/{r.AddedOn.Month}/{r.AddedOn.Year}"));

            CreateMap<Recipe, RecipeEditModel>()
                .ForMember(m => m.Author, c => c.MapFrom(r => r.Author.UserName))
                .ForMember(m => m.Course, c => c.MapFrom(r => r.Course.Name))
                .ForMember(m => m.Category, c => c.MapFrom(r => r.Category.Name));

            CreateMap<User, UserViewModel>()
                .ForMember(u => u.Username, c => c.MapFrom(u => u.UserName));

            CreateMap<RecipeIngredient, IngredientEditModel>()
                .ForMember(m => m.Name, c => c.MapFrom(r => r.Ingredient.Name))
                .ForMember(m => m.Measurement, c => c.MapFrom(r => r.Measurement.Name));
        }
    }
}
