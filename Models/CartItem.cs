using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ECommercePlatform.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual  Product Product { get; set; }  // Changed to virtual for lazy loading

        public int Quantity { get; set; }

        [ForeignKey("User")]
        public required string UserId { get; set; }  // Remove default value
        public virtual  IdentityUser User { get; set; }
    }
}