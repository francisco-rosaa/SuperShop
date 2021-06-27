using System.Collections.Generic;
using System.Threading.Tasks;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public interface IRepository
    {
        IEnumerable<Product> GetProducts();

        Product GetProduct(int id);

        void AddProduct(Product product);

        void UpdateProduct(Product product);

        void RemoveProduct(Product product);

        Task<bool> SaveAllAsync();

        bool ProductExists(int id);
    }
}
