using BlackSwanPlat.Areas.Identity.Data;
using static BlackSwanPlat.Areas.Identity.Data.Order;

namespace BlackSwanPlat.ViewModel
{
    public class OrderViwModel
    {
        public int orderid { get; set; }
       
        public string fullname { get; set; }

        public string address { get; set; }

        public string productName { get; set; }
        public float productPrice { get; set; }



    }
}
