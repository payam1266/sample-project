using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;

namespace BlackSwanPlat.Areas.Identity.Data
{
    public class Product
    {
        public int id { get; set; }
      
        public string name { get; set; }
       
        public int count { get; set; }
       
        public float price { get; set; }
       
        public DateTime date { get; set; }
        public string description { get; set; }
        
        public string size { get; set; }
       
        public string color { get; set; }
        public bool IsAvalaible { get; set; }
        public ICollection<ProductImages> productImages { get; set; }
        public string sellerid { get; set; }
        [ForeignKey("sellerid")]
        public int brandId { get; set; }
        [ForeignKey("brandId")]
        public Brand brand { get; set; }
        public int cityId { get; set; }
        [ForeignKey("cityId")]
        public City city { get; set; }
  
        public int productCategoryId { get; set; }
        [ForeignKey("productCategoryId")]
        public ProductCategory productCategory { get; set; }
        public ICollection<OrderDetails>? orderDetails { get; set; }
   
        public ApplicationUser seller { get; set; }
      

    }
}
