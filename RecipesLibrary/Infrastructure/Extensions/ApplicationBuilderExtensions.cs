using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipesLibrary.Data;
using RecipesLibrary.Infrastructure;
using RecipesLibrary.Infrastructure.MapperProfiles;
using RecipesLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipesLibrary.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
        {
            using(var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<RecipesDbContext>().Database.Migrate();

                var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                var rolesManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                Task.Run(async () =>
                {
                    var adminName = GlobalConstants.AdministratorRole;
                    var roleExists = await rolesManager.RoleExistsAsync(adminName);
                    if (!roleExists)
                    {
                        await rolesManager.CreateAsync(new IdentityRole
                        {
                            Name = adminName
                        });
                    }

                    var userRole = GlobalConstants.UserRole;
                    var userRoleExists = await rolesManager.RoleExistsAsync(userRole);
                    if (!userRoleExists)
                    {
                        await rolesManager.CreateAsync(new IdentityRole
                        {
                            Name = userRole
                        });
                    }

                    var adminUser = await userManager.FindByNameAsync(adminName);
                    if(adminUser == null)
                    {
                        adminUser = new User
                        {
                            Email = "admin@rl.com",
                            UserName = "admin"
                        };

                        await userManager.CreateAsync(adminUser, "admin12");

                        await userManager.AddToRoleAsync(adminUser, adminName);
                    }
                })
                .GetAwaiter()
                .GetResult();

                Mapper.Initialize(c =>
                {
                    c.AddProfile<MapperProfile>();
                });
            }

            return app;
        }
    }
}
