using asmc6.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace asmc6.Models
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Users.Any()) return; // tránh seed lại nhiều lần

            // 1. User
            var users = new List<User>
{
    new User { FullName = "Admin", Username = "admin", Email = "admin@gmail.com", Password = "123", Role = "Admin" },
    new User { FullName = "Nguyen Van A", Username = "nguyenvana", Email = "a@gmail.com", Password = "123", Role = "Customer" }
};

            db.Users.AddRange(users);

            // 2. Category
            var categories = new List<Category>
            {
                new Category { Name = "Burger" },
                new Category { Name = "Pizza" },
                new Category { Name = "Đồ uống" }
            };
            db.Categories.AddRange(categories);
            db.SaveChanges(); // cần lưu để có ID cho khóa ngoại

            // 3. FoodItem
            var foods = new List<FoodItem>
            {
                new FoodItem { Name = "Burger bò", Description = "Thịt bò nướng, rau xà lách", Price = 50000, CategoryId = categories[0].Id },
                new FoodItem { Name = "Pizza hải sản", Description = "Tôm, mực, phô mai", Price = 80000, CategoryId = categories[1].Id },
                new FoodItem { Name = "Trà sữa", Description = "Trân châu đường đen", Price = 30000, CategoryId = categories[2].Id }
            };
            db.FoodItems.AddRange(foods);
            db.SaveChanges();

            // 4. Combo
            var combo = new Combo
            {
                Name = "Combo tiết kiệm",
                Price = 100000,
                ComboItems = new List<ComboItem>
                {
                    new ComboItem { FoodItemId = foods[0].Id },
                    new ComboItem { FoodItemId = foods[2].Id }
                }
            };
            db.Combos.Add(combo);
            db.SaveChanges();

            // 5. Order + OrderItem
            var order = new Order
            {
                UserId = users[1].Id,
                OrderDate = DateTime.Now,
                Status = "Đang giao",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { FoodItemId = foods[0].Id, Quantity = 2 },
                    new OrderItem { FoodItemId = foods[1].Id, Quantity = 1 }
                }
            };
            db.Orders.Add(order);

            db.SaveChanges();
        }
    }
}
