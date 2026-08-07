namespace Weatherapplication.Models
{
    public class CustomerMaster
    {
        public int Id { get; set; }

        public string? CustomerCode { get; set; }

        public string? CustomerName { get; set; }

        public string? CompanyName { get; set; }

        public string? ContactPerson { get; set; }

        public string? MobileNo { get; set; }

        public string? AlternateMobile { get; set; }

        public string? Email { get; set; }

        public string? GSTNo { get; set; }

        public string? PANNo { get; set; }

        public string? AadhaarNo { get; set; }

        public string? CustomerType { get; set; }

        public decimal? CreditLimit { get; set; }

        public string? PaymentTerms { get; set; }

        public decimal? OpeningBalance { get; set; }

        public string? BalanceType { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? Pincode { get; set; }

        public bool IsActive { get; set; }

        public int? UserId { get; set; }
        public int? partytype { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
