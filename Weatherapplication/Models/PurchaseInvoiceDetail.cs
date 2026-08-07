using System.ComponentModel.DataAnnotations.Schema;

namespace Weatherapplication.Models
{
    public class PurchaseInvoiceDetail
    {
        public int Id { get; set; }
        public string? PurchaseInvoiceNo { get; set; }
        public string? Reference { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? PurchaseInvoiceDate { get; set; }
        public double? TotalAmount { get; set; }
        public double? TotalTax { get; set; }
        public double? NetAmount { get; set; }
        public int? UserId { get; set; }
        public int? poid { get; set; }

        // View binding ke liye
        [NotMapped]
        public List<PurchaseInvoiceItemDetail> PurchaseInvoiceItem { get; set; } = new();
    }
}
