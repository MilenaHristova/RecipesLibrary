using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Services.Contracts
{
    public interface IService
    {
        List<string> All();

        void Add(string name);
    }
}
