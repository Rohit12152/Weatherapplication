using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Weatherapplication.Models
{
    public class PurchaseInvoiceItemDetail
    {
        public int Id { get; set; }

        public int? PurchaseInvoiceId { get; set; }
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

        [JsonIgnore]   
        public virtual PurchaseInvoiceDetail? PurchaseInvoice { get; set; }
    }
}
