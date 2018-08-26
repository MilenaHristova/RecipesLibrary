using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Infrastructure
{
    public class Alert
    {
        public string Message;
        public string Type;

        public Alert(string message, string type)
        {
            this.Message = message;
            this.Type = type;
        }
    }
}
