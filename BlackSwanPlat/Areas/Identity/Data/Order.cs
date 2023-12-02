using System.ComponentModel.DataAnnotations;

namespace BlackSwanPlat.Areas.Identity.Data
{
    public class Order
    {
        public enum OrderStatus
        {
            PendingPayment,
            Paid,
            InPreparation,
            Canceled
        }

        public int id { get; set; }

        public string fullname { get; set; }
        public string customerid { get; set; }
      

       public string paymentmethod { get; set; }

        public string address { get; set; }
        public string phone{ get; set; }
        public string city { get; set; }
       
        public string postalcode { get; set; }
        public DateTime dateTime { get; set; }
        public float totalPrice { get; set; }
        public OrderStatus status{ get; set; }
        public ICollection<OrderDetails>? orderDetails { get; set; }
    }
}
