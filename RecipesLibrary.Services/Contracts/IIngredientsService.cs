using RecipesLibrary.Services.Models.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Services.Contracts
{
    public interface IIngredientsService
    {
        List<IngredientModel> All();

        void Add(string name);

        void Delete(int id);

        IngredientModel GetById(int id);

        void Edit(IngredientModel model);
    }
}
