using System.ComponentModel.DataAnnotations.Schema;

namespace Weatherapplication.Models
{
    public class PurchaseItemDetail
    {
        public int Id { get; set; }

        public int? PoId { get; set; }
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

        public virtual PurchaseDetail Po { get; set; }
    }
}
