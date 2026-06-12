namespace Weatherapplication.Models
{
    public class SalesItemDetail
    {
        public int Id { get; set; }

        public int SalesId { get; set; }

        public int ItemId { get; set; }

        public decimal? Qty { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Amount { get; set; }

        public decimal? GST { get; set; }

        public decimal? TaxPercent { get; set; }

        public decimal? TaxAmount { get; set; }

        public decimal? TotalAmount { get; set; }

        public virtual SalesDetail Sales { get; set; }
    }
}
