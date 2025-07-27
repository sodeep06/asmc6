namespace asmc6.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // "Pending", "Delivering", "Delivered"
        public string? PaymentMethod { get; set; } // Momo, ZaloPay...

        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
