namespace tp24
{
    
    public class Receivable
    {
        public string Reference { get; set; }
        public string CurrencyCode { get; set; }
        public string IssueDate { get; set; }
        public double OpeningValue { get; set; }
        public double PaidValue { get; set; }
        public string DueDate { get; set; }
        public string? ClosedDate { get; set; }
        public bool Cancelled { get; set; }
        public string DebtorName { get; set; }
        public string DebtorReference { get; set; }
        public string DebtorAddress1 { get; set; }
        public string? DebtorAddress2 { get; set; }
        public string? DebtorTown { get; set; }
        public string? DebtorState { get; set; }
        public string? DebtorZip { get; set; }
        public string DebtorCountryCode { get; set; }
        public string? DebtorRegistrationNumber { get; set; }
    }
}