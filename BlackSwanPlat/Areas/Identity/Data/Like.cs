using System.ComponentModel.DataAnnotations.Schema;

namespace BlackSwanPlat.Areas.Identity.Data
{
    public class Like
    {
       
        public int Id { get; set; }


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }


        public int productid { get; set; }
        [ForeignKey("productid")]
        public Product Product { get; set; }
    }
}
