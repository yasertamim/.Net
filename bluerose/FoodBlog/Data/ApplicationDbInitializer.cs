using System;
using Microsoft.AspNetCore.Identity;

using FoodBlog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodBlog.Data
{
    public static class ApplicationDbInitializer
    {

        public static void Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um,
            RoleManager<IdentityRole> rm)
        {


            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var adminRole = new IdentityRole("Admin");
            rm.CreateAsync(adminRole).Wait();

            var admin = new ApplicationUser
            {
                UserName = "admin@uia.no",
                Email = "admin@uia.no",
                Nickname = "knutinne",
                EmailConfirmed = true
            };

            um.CreateAsync(admin, "Password1.").Wait();
            um.AddToRoleAsync(admin, "Admin").Wait();


            var user = new ApplicationUser
            {
                UserName = "user@uia.no",
                Email = "user@uia.no",
                Nickname = "knut",
                EmailConfirmed = true
            };
            um.CreateAsync(user, "Password1.").Wait();
            
            












        }
    }
}