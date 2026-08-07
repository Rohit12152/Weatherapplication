using System.ComponentModel.DataAnnotations.Schema;

namespace Weatherapplication.Models
{
    public class SalesItemDetail
    {
        public int Id { get; set; }

        public int? SalesId { get; set; }
        [NotMapped]
        public int? categoryid { get; set; }
        public int ItemId { get; set; }

        public double? Qty { get; set; }

        public double? Rate { get; set; }

        public double? Amount { get; set; }

        public double? GST { get; set; }

        public double? TaxPercent { get; set; }

        public double? TaxAmount { get; set; }

        public double? TotalAmount { get; set; }

        public virtual SalesDetail Sales { get; set; }
    }
}
