using System.ComponentModel.DataAnnotations.Schema;

namespace Weatherapplication.Models
{
    public class DeliveryOrderDetail
    {
        public int Id { get; set; }
        public string? DONo { get; set; }
        public int? SOId { get; set; }
        public string? Reference { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? DODate { get; set; }
        public double? TotalAmount { get; set; }
        public double? TotalTax { get; set; }
        public double? NetAmount { get; set; }
        public int? UserId { get; set; }

        // View binding ke liye
        [NotMapped]
        public List<DeliveryOrderItemDetail> DeliveryOrderItem { get; set; } = new();
    }
}
