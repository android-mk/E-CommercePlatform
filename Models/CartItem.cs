using Microsoft.AspNetCore.Identity;

namespace ECommercePlatform.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; } // Make nullable
        public int Quantity { get; set; }
        public string UserId { get; set; } = string.Empty; // Initialize with default value
        public IdentityUser? User { get; set; } // Make nullable
    }
}