namespace asmc6.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<FoodItem>? FoodItems { get; set; }
    }
}
