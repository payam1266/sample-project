using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlackSwanPlat.Areas.Identity.Data
{
    public class Comment
    {
   
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

       
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

       
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }


}
