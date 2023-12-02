namespace BlackSwanPlat.Areas.Identity.Data
{
    public class ProductCategory
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
        public ICollection<Product>? products { get; set; }
    }
}
