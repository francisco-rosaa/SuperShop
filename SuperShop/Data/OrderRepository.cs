using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;

namespace SuperShop.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper)
            : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<IQueryable<Order>> GetOrderAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Admin"))
            {
                return _context.Orders
                    .Include(x => x.User)
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                    .OrderByDescending(x => x.OrderDate);
            }

            return _context.Orders
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .Where(x => x.User == user)
                .OrderByDescending(x => x.OrderDate);
        }

        public async Task<IQueryable<OrderDetailTemp>> GetDetailsTempAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null;
            }

            return _context.OrderDetailsTemp
                .Include(x => x.Product)
                .Where(x => x.User == user)
                .OrderBy(x => x.Product.Name);
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return;
            }

            var product = await _context.Products.FindAsync(model.ProductId);

            if (product == null)
            {
                return;
            }

            var orderDetailTemp = await _context.OrderDetailsTemp
                .Where(x => x.User == user && x.Product == product)
                .FirstOrDefaultAsync();

            if (orderDetailTemp == null)
            {
                orderDetailTemp = new OrderDetailTemp
                {
                    User = user,
                    Product = product,
                    Price = product.Price,
                    Quantity = model.Quantity
                };

                _context.OrderDetailsTemp.Add(orderDetailTemp);
            }
            else
            {
                orderDetailTemp.Quantity += model.Quantity;

                _context.OrderDetailsTemp.Update(orderDetailTemp);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);

            if (orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;

            if (orderDetailTemp.Quantity > 0)
            {
                _context.OrderDetailsTemp.Update(orderDetailTemp);

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);

            if (orderDetailTemp == null)
            {
                return;
            }

            _context.OrderDetailsTemp.Remove(orderDetailTemp);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmOrderAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return false;
            }

            var orderTemps = await _context.OrderDetailsTemp
                .Include(x => x.Product)
                .Where(x => x.User == user)
                .ToListAsync();

            if (orderTemps == null || orderTemps.Count == 0)
            {
                return false;
            }

            var details = orderTemps.Select(x => new OrderDetail
            {
                Product = x.Product,
                Price = x.Price,
                Quantity = x.Quantity
            }).ToList();

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                User = user,
                Items = details
            };

            await CreateAsync(order);

            _context.OrderDetailsTemp.RemoveRange(orderTemps);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task DeliveryOrder(DeliveryViewModel model)
        {
            var order = await _context.Orders.FindAsync(model.Id);

            if (order == null)
            {
                return;
            }

            order.DeliveryDate = model.DeliveryDate;

            _context.Orders.Update(order);

            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }
    }
}
