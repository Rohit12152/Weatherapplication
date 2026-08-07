using System.ComponentModel.DataAnnotations.Schema;

namespace Weatherapplication.Models
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        public string? PoNo { get; set; }
        public string? Reference { get; set; }
        public int? StudentId { get; set; }
        public DateTime? PoDate { get; set; }
        public double? TotalAmount { get; set; }
        public double? TotalTax { get; set; }
        public double? NetAmount { get; set; }
        public int? UserId { get; set; }

        // View binding ke liye
        [NotMapped]
        public List<PurchaseItemDetail> PurchaseItems { get; set; } = new();
    }
}
