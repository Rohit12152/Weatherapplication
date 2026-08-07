using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Weatherapplication.Models;

public class DeliveryOrderItemDetail
{
    public int Id { get; set; }

    public int? DOId { get; set; }

    [ForeignKey(nameof(DOId))]
    [JsonIgnore]
    public virtual DeliveryOrderDetail? DeliveryOrder { get; set; }
    [NotMapped]
    public int? categoryid { get; set; }

    public int ItemId { get; set; }

    [NotMapped]
    public double? SOQty { get; set; }

    public double? Qty { get; set; }
    public double? Rate { get; set; }
    public double? Amount { get; set; }
    public double? GST { get; set; }
    public double? TaxPercent { get; set; }
    public double? TaxAmount { get; set; }
    public double? TotalAmount { get; set; }
}