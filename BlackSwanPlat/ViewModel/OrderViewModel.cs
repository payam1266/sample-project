using static BlackSwanPlat.Areas.Identity.Data.Order;

namespace BlackSwanPlat.ViewModel
{
    public class OrderViewModel
    {
       
        public string fullname { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string postalcode { get; set; }
        public float totalPrice { get; set; }
        public string paymentmethod { get; set; }
        public string city { get; set; }
        public DateTime dateTime { get; set; }
        public OrderStatus status { get; set; }
    }
}
