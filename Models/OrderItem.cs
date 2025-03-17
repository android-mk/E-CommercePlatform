using System.ComponentModel.DataAnnotations.Schema;

namespace ECommercePlatform.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; } // Foreign key to Orders
        public Order Order { get; set; }= null!; // Navigation property for Order

        [ForeignKey("Product")]
        public int ProductId { get; set; } // Foreign key to Products
        public Product Product { get; set; }= null!; // Navigation property for Product

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}