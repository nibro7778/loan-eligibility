namespace LoanEligibilityService.Application.DTOs
{
    public class LoanEligibilityResponse
    {
        public bool IsEligible { get; set; }
        public List<string> Reasons { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
    }
}
