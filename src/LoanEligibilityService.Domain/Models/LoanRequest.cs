namespace LoanEligibilityService.Domain.Models
{
    public class LoanRequest
    {
        public string ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public int Age { get; set; }
        public int CreditScore { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal LoanAmount { get; set; }

        public LoanRequest(
            string applicantId,
            string applicantName,
            int age,
            int creditScore,
            decimal monthlyIncome,
            decimal loanAmount)
        {
            ApplicantId = applicantId;
            ApplicantName = applicantName;
            Age = age;
            CreditScore = creditScore;
            MonthlyIncome = monthlyIncome;
            LoanAmount = loanAmount;
        }
    }
}
