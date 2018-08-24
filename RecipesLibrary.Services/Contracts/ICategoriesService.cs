using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Services.Contracts
{
    public interface ICategoriesService:IService
    {
        bool ExistsByName(string name);
    }
}
