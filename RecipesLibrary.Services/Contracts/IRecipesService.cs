using RecipesLibrary.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RecipesLibrary.Services
{
    public interface IRecipesService
    {
        List<RecipeViewModel> All();

        Task AddRecipe(RecipeAddModel model, string currentUserName);

        List<RecipeWithIngredients> AllRecipesWithIngredients();

        List<RecipeViewModel> Last(int count);

        List<RecipeViewModel> GetByUserId(string userId);

        RecipeDetailsModel GetById(int id);

        List<RecipeViewModel> GetByCategory(string category);

        RecipeEditModel GetForEdit(int id);

        void Delete(int id);

        void Edit(RecipeEditModel model);

        List<RecipeViewModel> Search(string searchTerm);
    }
}
