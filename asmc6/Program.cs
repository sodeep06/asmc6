// Program.cs (đã chỉnh sửa theo yêu cầu Y1 và thêm kiểm tra quyền Admin qua header)

using asmc6.Data;
using asmc6.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=FastFoodC6;User Id=sa;Password=12345678;TrustServerCertificate=True;"));

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});
var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbInitializer.Seed(db);
}

#region USER
// Đăng ký
app.MapPost("/api/users/register", async (AppDbContext db, User user) =>
{
    var exists = await db.Users.AnyAsync(u => u.Email == user.Email);
    if (exists) return Results.BadRequest("Email đã tồn tại");

    user.Role = "Customer";
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok(user);
});

// Đăng nhập
app.MapPost("/api/users/login", async (AppDbContext db, LoginDto login) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);
    return user != null ? Results.Ok(user) : Results.Unauthorized();
});

// Danh sách người dùng (Admin)
app.MapGet("/api/users", async (AppDbContext db, HttpRequest req) =>
{
    var email = req.Headers["email"].ToString();
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null || user.Role != "Admin") return Results.Unauthorized();

    return Results.Ok(await db.Users.ToListAsync());
});
#endregion

#region FOOD
// Danh sách món ăn
app.MapGet("/api/foods", async (AppDbContext db) => await db.FoodItems.Include(f => f.Category).ToListAsync());

// Xem chi tiết món ăn
app.MapGet("/api/foods/{id}", async (AppDbContext db, int id) =>
{
    var food = await db.FoodItems.Include(f => f.Category).FirstOrDefaultAsync(f => f.Id == id);
    return food is null ? Results.NotFound() : Results.Ok(food);
});

// Thêm món ăn (Admin)
app.MapPost("/api/foods", async (AppDbContext db, FoodItem food, HttpRequest req) =>
{
    var email = req.Headers["email"].ToString();
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null || user.Role != "Admin") return Results.Unauthorized();

    db.FoodItems.Add(food);
    await db.SaveChangesAsync();
    return Results.Ok(food);
});

// Sửa món ăn (Admin)
app.MapPut("/api/foods/{id}", async (AppDbContext db, int id, FoodItem input, HttpRequest req) =>
{
    var email = req.Headers["email"].ToString();
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null || user.Role != "Admin") return Results.Unauthorized();

    var food = await db.FoodItems.FindAsync(id);
    if (food == null) return Results.NotFound();

    food.Name = input.Name;
    food.Price = input.Price;
    food.Description = input.Description;
    food.CategoryId = input.CategoryId;
    await db.SaveChangesAsync();
    return Results.Ok(food);
});

// Xóa món ăn (Admin)
app.MapDelete("/api/foods/{id}", async (AppDbContext db, int id, HttpRequest req) =>
{
    var email = req.Headers["email"].ToString();
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null || user.Role != "Admin") return Results.Unauthorized();

    var food = await db.FoodItems.FindAsync(id);
    if (food == null) return Results.NotFound();
    db.FoodItems.Remove(food);
    await db.SaveChangesAsync();
    return Results.Ok();
});
#endregion

#region COMBO
app.MapGet("/api/combos", async (AppDbContext db) => await db.Combos.Include(c => c.ComboItems).ThenInclude(i => i.FoodItem).ToListAsync());

app.MapPost("/api/combos", async (AppDbContext db, Combo combo, HttpRequest req) =>
{
    var email = req.Headers["email"].ToString();
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null || user.Role != "Admin") return Results.Unauthorized();

    db.Combos.Add(combo);
    await db.SaveChangesAsync();
    return Results.Ok(combo);
});
#endregion

#region ORDER
// Đặt hàng
app.MapPost("/api/orders", async (AppDbContext db, Order order) =>
{
    order.OrderDate = DateTime.Now;
    order.Status = "Chưa giao";
    db.Orders.Add(order);
    await db.SaveChangesAsync();
    return Results.Ok(order);
});

// Danh sách đơn hàng theo người dùng
app.MapGet("/api/orders/user/{userId}", async (AppDbContext db, int userId) =>
    await db.Orders.Include(o => o.OrderItems).ThenInclude(i => i.FoodItem).Where(o => o.UserId == userId).ToListAsync());
#endregion

#region CATEGORY
app.MapGet("/api/categories", async (AppDbContext db) => await db.Categories.ToListAsync());

app.MapPost("/api/categories", async (AppDbContext db, Category cat, HttpRequest req) =>
{
    var email = req.Headers["email"].ToString();
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null || user.Role != "Admin") return Results.Unauthorized();

    db.Categories.Add(cat);
    await db.SaveChangesAsync();
    return Results.Ok(cat);
});
#endregion

app.Run();

// DTO cho đăng nhập
public record LoginDto(string Email, string Password);
