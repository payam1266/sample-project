using BlackSwanPlat.Areas.Identity.Data;

namespace BlackSwanPlat.ViewModel
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public float price { get; set; }
        public string description { get; set; }
        public string size { get; set; }

        public string color { get; set; }
        public List<ProductImages> Images { get; set; } 
    }
}
