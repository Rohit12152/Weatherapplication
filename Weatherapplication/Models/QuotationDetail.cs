namespace Weatherapplication.Models
{
    public class QuotationDetail
    {
        public int Id { get; set; }

        public string? QuotationNo { get; set; }

        public int? StudentId { get; set; }
        public StudentDetails Student { get; set; }

        public DateTime QuotationDate { get; set; }

        public double? TotalAmount { get; set; }

        public double? TotalTax { get; set; }

        public double? NetAmount { get; set; }
        public int? UserId { get; set; }

        //public virtual ICollection<QuotationItemDetail> QuotationDetails { get; set; }
        //    = new List<QuotationItemDetail>();
    }
}

