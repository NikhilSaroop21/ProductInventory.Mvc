using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ProductInventory.Mvc.Models;

namespace ProductInventory.Mvc.Services
{
    // Thread-safe in-memory store for app lifetime
    public class InventoryService : IInventoryService
    {
        private readonly ConcurrentDictionary<int, Product> _store = new();

        public InventoryService()
        {
            // Seed at least 10 products (IDs & prices can be adjusted)
            var seed = new[]
            {
                new Product{ ProductID=101, Name="Laptop",        Price=5000m },
                new Product{ ProductID=102, Name="JBL Speaker",   Price=8000m },
                new Product{ ProductID=103, Name="Tablet",        Price=2300m },
                new Product{ ProductID=104, Name="Smartphone",    Price=4500m },
                new Product{ ProductID=105, Name="Headphones",    Price=1200m },
                new Product{ ProductID=106, Name="Gaming Mouse",  Price= 650m },
                new Product{ ProductID=107, Name="Keyboard",      Price= 850m },
                new Product{ ProductID=108, Name="Monitor 24\"",  Price=2200m },
                new Product{ ProductID=109, Name="External SSD",  Price=1700m },
                new Product{ ProductID=110, Name="Webcam",        Price= 900m },
            };
            foreach (var p in seed) _store[p.ProductID] = p;
        }

        public int Count => _store.Count;

        public IReadOnlyCollection<Product> GetAll()
            => _store.Values.OrderBy(p => p.ProductID).ToList();

        public IReadOnlyCollection<Product> Search(string? query)
        {
            if (string.IsNullOrWhiteSpace(query)) return GetAll();

            query = query.Trim();
            // try parse ID
            if (int.TryParse(query, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
            {
                return _store.TryGetValue(id, out var p) ? new[] { p } : new Product[0];
            }

            // name contains (case-insensitive)
            return _store.Values
                         .Where(p => p.Name.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                         .OrderBy(p => p.ProductID)
                         .ToList();
        }

        public bool TryGet(int productId, out Product? product)
            => _store.TryGetValue(productId, out product);

        public bool Add(Product product)
            => _store.TryAdd(product.ProductID, product);

        public bool Update(Product product)
        {
            if (!_store.ContainsKey(product.ProductID)) return false;
            _store[product.ProductID] = product;
            return true;
        }

        public bool Delete(int productId)
            => _store.TryRemove(productId, out _);
    }
}
