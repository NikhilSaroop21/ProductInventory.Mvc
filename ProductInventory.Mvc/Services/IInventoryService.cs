using System.Collections.Generic;
using ProductInventory.Mvc.Models;

namespace ProductInventory.Mvc.Services
{
    public interface IInventoryService
    {
        IReadOnlyCollection<Product> GetAll();
        IReadOnlyCollection<Product> Search(string? query); // name or ID
        bool TryGet(int productId, out Product? product);

        // Returns false if ProductID exists (for Add) or missing (for Update/Delete)
        bool Add(Product product);
        bool Update(Product product);
        bool Delete(int productId);

        int Count { get; }
    }
}
