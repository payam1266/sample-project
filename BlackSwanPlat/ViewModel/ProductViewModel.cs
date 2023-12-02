using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace BlackSwanPlat.ViewModel
{
    public class ProductViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(10, ErrorMessage = "The Name field cannot exceed 10 characters.")]
        [Microsoft.AspNetCore.Mvc.Remote("checkProductName","Admin",ErrorMessage ="This Name Is Exist.")]
        public string name { get; set; }
        [Required(ErrorMessage = "The Count field is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Count must be greater than 0.")]
        public int count { get; set; }
        [Required(ErrorMessage = "The Price field is required.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public float price { get; set; }
        [Required(ErrorMessage = "The Date field is required.")]
        public DateTime date { get; set; }
        public string description { get; set; }
        [Required(ErrorMessage = "The Size field is required.")]
        public string size { get; set; }
        [Required(ErrorMessage = "The Size field is required.")]
        public string color { get; set; }
        public int catrgoryid { get; set; }
        public int brandid { get; set; }
        public int cityid { get; set; }

        public string sellerid { get; set; }
        public bool IsAvalaible { get; set; }
      
        public ICollection<ImagesViewModel> imagesViewModels { get; set; }
        public  List<int> imageDeletId { get; set; }
        public List<IFormFile> img { get; set; }
    }
}
