namespace asmc6.Models
{
    public class Combo
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public ICollection<ComboItem>? ComboItems { get; set; }
    }
}
