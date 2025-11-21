using Swashbuckle.AspNetCore.Filters;
using ECommerceApi.Entities;

namespace ECommerceApi
{
    // Example สำหรับ list ของ Product
    public class ProductListExample : IExamplesProvider<List<Product>>
    {
        public List<Product> GetExamples()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = Guid.Parse("d6e2a82d-8c37-4030-8682-a585c91ab105"),
                    Name = "Product A",
                    Description = "Product A",
                    Price = 500.00m,
                    Stock = 10
                },
                new Product
                {
                    Id = Guid.Parse("83cbbc55-a151-4af0-aac2-34a868ee3a06"),
                    Name = "Product B",
                    Description = "Product B",
                    Price =  500.00m,
                    Stock = 5
                },
                new Product
                {
                    Id = Guid.Parse("071271a0-a62b-411d-98af-b14bab9d00b6"),
                    Name = "Product C",
                    Description = "Product C",
                    Price =  500.00m,
                    Stock = 5
                }
            };
        }
    }
}
