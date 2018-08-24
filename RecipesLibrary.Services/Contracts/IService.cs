using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesLibrary.Services.Contracts
{
    public interface IService
    {
        List<string> All();

        Dictionary<int, string> AllWithIds();

        void Add(string name);

        void Delete(int id);
    }
}
