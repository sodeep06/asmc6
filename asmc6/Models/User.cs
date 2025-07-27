using System.ComponentModel.DataAnnotations;

namespace asmc6.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Phone { get; set; } = "";

        [Required]
        public string Address { get; set; } = "";

        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        public string Role { get; set; } = "Customer"; // "Admin" hoặc "Customer"

        public ICollection<Order>? Orders { get; set; }
    }
}
