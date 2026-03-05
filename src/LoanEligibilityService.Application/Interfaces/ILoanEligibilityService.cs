using LoanEligibilityService.Application.DTOs;

namespace LoanEligibilityService.Application.Interfaces
{
    public interface ILoanEligibilityService
    {
        Task<LoanEligibilityResponse> CheckEligibilityAsync(LoanEligibilityRequest request);
    }
}
