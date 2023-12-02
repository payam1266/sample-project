using BlackSwanPlat.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlackSwanPlat.ViewModel
{
    public class OrderDetailsViewModel
    {



       


        public string productName { get; set; }
        public float productPrice { get; set; }
        public int quantity { get; set; }
        public float subtotal { get; set; }
    }
}
