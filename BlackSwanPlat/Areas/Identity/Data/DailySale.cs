namespace BlackSwanPlat.Areas.Identity.Data
{
    public class DailySale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalSales { get; set; }
    }
}
