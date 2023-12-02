using System.ComponentModel.DataAnnotations.Schema;

namespace BlackSwanPlat.Areas.Identity.Data
{
    public class ProductImages
    {
        public int id { get; set; }
        public byte[] thumbnail { get; set; }
        public byte[] img { get; set; } 
        public int productid { get; set; }
        [ForeignKey("productid")]
        public Product products { get; set; }
      
    }
}
