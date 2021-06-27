using System;
using System.Linq;
using System.Threading.Tasks;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Products.Any())
            {
                await Task.Run(() => 
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        AddProduct($"Product {i}");
                    }
                });

                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string productName)
        {
            _context.Products.Add(new Product
            {
                Name = productName,
                Price = _random.Next(5, 1001),
                IsAvailable = true,
                Stock = _random.Next(10, 101)
            });
        }
    }
}
