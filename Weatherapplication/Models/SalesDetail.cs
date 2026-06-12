using System.ComponentModel.DataAnnotations.Schema;

namespace Weatherapplication.Models
{
    public class SalesDetail
    {
        public int Id { get; set; }
        public string SalesNo { get; set; }
        public int? QuotationId { get; set; }
        public string? ReferenceQuotationNo { get; set; }
        public int? StudentId { get; set; }
        public DateTime? SalesDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalTax { get; set; }
        public decimal? NetAmount { get; set; }
        public int? UserId { get; set; }

        // View binding ke liye
        [NotMapped]
        public List<SalesItemDetail> SalesItems { get; set; } = new();
    }
}
