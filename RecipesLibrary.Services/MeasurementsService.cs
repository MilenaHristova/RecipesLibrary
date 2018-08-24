using RecipesLibrary.Data;
using RecipesLibrary.Models;
using RecipesLibrary.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecipesLibrary.Services
{
    public class MeasurementsService:IMeasurementsService
    {
        private readonly RecipesDbContext dbContext;

        public MeasurementsService(RecipesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Add(string name)
        {
            this.dbContext.Add(new Measurement
            {
                Name = name
            });

            this.dbContext.SaveChanges();
        }

        public List<string> All()
        {
            var res = this.dbContext
                .Measurements
                .Select(c => c.Name)
                .ToList();

            return res;
        }

        public Dictionary<int, string> AllWithIds()
        {
            var res = this.dbContext
                .Measurements
                .ToDictionary(c => c.Id, c => c.Name);

            return res;
        }

        public void Delete(int id)
        {
            var m = this.dbContext.Measurements.Find(id);

            this.dbContext.Remove(m);
            this.dbContext.SaveChanges();
        }
    }
}
