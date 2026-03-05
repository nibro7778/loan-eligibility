namespace LoanEligibilityService.Application.DTOs
{
    public class LoanEligibilityRequest
    {
        public string ApplicantId { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public int Age { get; set; }
        public int CreditScore { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal LoanAmount { get; set; }
    }
}
