using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
            await _context.Database.EnsureCreatedAsync();

            var user = await _userHelper.GetUserByEmailAsync("frnuno@protonmail.com");

            if ( user == null)
            {
                user = new User 
                {
                    FirstName = "Francisco",
                    LastName = "Rosa",
                    Email = "frnuno@protonmail.com",
                    UserName = "frnuno",
                    PhoneNumber = "123456789"
                };

                var result = await _userHelper.AddUserAsync(user, "123");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create seeder user.");
                }
            }

            if (!_context.Products.Any())
            {
                await Task.Run(() => 
                {
                    for (int i = 1; i <= 20; i++)
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
