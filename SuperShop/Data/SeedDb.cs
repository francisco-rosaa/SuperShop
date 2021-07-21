using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using SuperShop.Helpers;

namespace SuperShop.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            var user = await _userHelper.GetUserByEmailAsync("frnuno@protonmail.com");

            if (!_context.Countries.Any())
            {
                var cities = new List<City>();

                cities.Add(new City { Name = "Lisboa" });
                cities.Add(new City { Name = "Porto" });
                cities.Add(new City { Name = "Faro" });

                _context.Countries.Add(new Country { Name = "Portugal", Cities = cities });
            }

            await _context.SaveChangesAsync();

            if ( user == null)
            {
                user = new User
                {
                    FirstName = "Francisco",
                    LastName = "Rosa",
                    Email = "frnuno@protonmail.com",
                    UserName = "frnuno@protonmail.com",
                    PhoneNumber = "123456789",
                    Adress = "Rua Jau 33",
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create seeder user.");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_context.Products.Any())
            {
                await Task.Run(() => 
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        AddProduct($"Product {i}", user);
                    }
                });

                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string productName, User user)
        {
            _context.Products.Add(new Product
            {
                Name = productName,
                Price = _random.Next(5, 1001),
                IsAvailable = true,
                Stock = _random.Next(10, 101),
                User = user
            });
        }
    }
}
