using System.ComponentModel.DataAnnotations;

namespace Weatherapplication.Models
{
    public class ItemMaster
    {
        [Key]
        public int Id { get; set; }

        public string? ItemCode { get; set; }

        public string? ItemName { get; set; }

        public string? Category { get; set; }

        public string? Unit { get; set; }

        public decimal PurchaseRate { get; set; }

        public decimal SaleRate { get; set; }

        public decimal GST { get; set; }

        public int OpeningStock { get; set; }

        public int MinStock { get; set; }

        public string? Brand { get; set; }

        public string? HSNCode { get; set; }

        public string? ItemDescription { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? categoryid { get; set; }
        public decimal? CurrentStock { get; set; }
    }
}