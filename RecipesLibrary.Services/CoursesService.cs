using RecipesLibrary.Data;
using RecipesLibrary.Models;
using RecipesLibrary.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecipesLibrary.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly RecipesDbContext dbContext;

        public CoursesService(RecipesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Add(string name)
        {
            this.dbContext
                .Add(new Course
                {
                    Name = name
                });

            this.dbContext.SaveChanges();
        }

        public List<string> All()
        {
            var res = this.dbContext
                .Courses
                .Select(c => c.Name)
                .ToList();

            return res;
        }

        public Dictionary<int, string> AllWithIds()
        {
            var res = this.dbContext
                .Courses
                .ToDictionary(c => c.Id, c => c.Name);

            return res;
        }

        public void Delete(int id)
        {
            var c = this.dbContext.Courses.Find(id);

            this.dbContext.Remove(c);
            this.dbContext.SaveChanges();
        }
    }
}
