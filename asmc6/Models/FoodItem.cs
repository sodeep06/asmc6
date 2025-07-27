namespace asmc6.Models
{
    public class FoodItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<ComboItem>? ComboItems { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
