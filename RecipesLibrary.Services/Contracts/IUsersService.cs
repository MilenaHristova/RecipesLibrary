using RecipesLibrary.Services.Models;
using System.Collections.Generic;

namespace RecipesLibrary.Services.Contracts
{
    public interface IUsersService
    {
        void Save(string username, int id);

        void Remove(string username, int id);

        UserViewModel GetByUsername(string username);

        List<RecipeViewModel> GetRecipesByUserName(string username);

        List<RecipeViewModel> GetSavedByUserName(string username);
    }
}
