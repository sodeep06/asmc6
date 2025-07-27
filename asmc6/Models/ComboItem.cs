namespace asmc6.Models
{
    public class ComboItem
    {
        public int Id { get; set; }

        public int ComboId { get; set; }
        public Combo? Combo { get; set; }

        public int FoodItemId { get; set; }
        public FoodItem? FoodItem { get; set; }
    }
}
