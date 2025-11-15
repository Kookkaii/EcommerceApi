using ECommerceApi.Entities;
using ECommerceApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.Migrate();

            // --- Seed Users ---
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Title = "Miss",
                        FirstName = "Admin",
                        LastName = "Test",
                        Email = "Admin@mail.com",
                        PasswordHash = PasswordHasher.HashPassword("p@ssw0rd")
                    }
                );
            }

            // --- Seed Products ---
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                     new Product
                     {
                         Name = "บะหมี่กึ่งสำเร็จรูปรสหมู",
                         Description = "บะหมี่กึ่งสำเร็จรูป รสหมู 60g",
                         Price = 15.00m,
                         Stock = 100
                     },
                      new Product
                      {
                          Name = "บะหมี่กึ่งสำเร็จรูปรสไก่",
                          Description = "บะหมี่กึ่งสำเร็จรูป รสไก่ 60g",
                          Price = 15.00m,
                          Stock = 200
                      },
                      new Product
                      {
                          Name = "บะหมี่กึ่งสำเร็จรูปรสต้มยำกุ้ง",
                          Description = "บะหมี่กึ่งสำเร็จรูป รสต้มยำกุ้ง 60g",
                          Price = 15.50m,
                          Stock = 300
                      },
                      new Product
                      {
                          Name = "บะหมี่กึ่งสำเร็จรูปรสผัก",
                          Description = "บะหมี่กึ่งสำเร็จรูป รสผัก 60g",
                          Price = 10.00m,
                          Stock = 200
                      }
                );
            }

            context.SaveChanges();
        }
    }
}