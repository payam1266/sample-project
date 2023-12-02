using System.ComponentModel.DataAnnotations.Schema;

namespace BlackSwanPlat.Areas.Identity.Data
{
    public class OrderDetails
    {
        public int id { get; set; }
        public int orderId { get; set; }
        [ForeignKey("orderId")]
        public Order Order { get; set; }
        public int productId { get; set; }
        [ForeignKey("productId")]
        public Product product { get; set; }
        public string productName { get; set; }
        public float productPrice { get; set; }
        public int quantity { get; set; }
        public float subtotal { get; set; }
    }
}
