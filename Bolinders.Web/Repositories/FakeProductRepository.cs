using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolinders.Web.Models;

namespace Bolinders.Web.Repositories
{
    public class FakeProductRepository : IProductRepository
    {
        public IEnumerable<Product> Products => new List<Product>
        {
            new Product {Name = "Product 1", Price = 100, Description = "Tada"},
            new Product {Name = "Product 2", Price = 200, Description = "Tada"},
            new Product {Name = "Product 3", Price = 300, Description = "Tada"},
            new Product {Name = "Product 4", Price = 400, Description = "Tada"},
            new Product {Name = "Product 5", Price = 500, Description = "Tada"},
            new Product {Name = "Product 6", Price = 600, Description = "Tada"},
            new Product {Name = "Product 7", Price = 700, Description = "Tada"},
            new Product {Name = "Product 8", Price = 800, Description = "Tada"},
            new Product {Name = "Product 9", Price = 900, Description = "Tada"},
            new Product {Name = "Product 10", Price = 1000, Description = "Tada"},
        };
    }
}
