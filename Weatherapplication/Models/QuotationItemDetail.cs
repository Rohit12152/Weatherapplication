using System.ComponentModel.DataAnnotations.Schema;
namespace Weatherapplication.Models
{
    public class QuotationItemDetail
    {
        public int Id { get; set; }

        public int? QuotationId { get; set; }
        [NotMapped]
        public int? categoryid { get; set; }

        public int ItemId { get; set; }

        public double? Qty { get; set; }

        public double? Rate { get; set; }

        public double? Amount { get; set; }

        public double? GST { get; set; }

        public double? TaxAmount { get; set; }

        public double? TotalAmount { get; set; }

        [ForeignKey("QuotationId")]
        public virtual QuotationDetail QuotationDetail { get; set; }
    }
}
