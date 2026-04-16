using LoanEligibilityService.Application.DTOs;
using LoanEligibilityService.Application.Interfaces;
using LoanEligibilityService.Domain.Models;

namespace LoanEligibilityService.Application.Services
{
    public class LoanEligibilityService : ILoanEligibilityService
    {
        private const int MinimumAge = 21;
        private const int MinimumCreditScore = 600;
        private const decimal MinimumMonthlyIncome = 2000;
        private const int LoanToIncomeMultiplier = 10;

        public Task<LoanEligibilityResponse> CheckEligibilityAsync(LoanEligibilityRequest request)
        {
            var response = new LoanEligibilityResponse();
            var reasons = new List<string>();

            var loanRequest = new LoanRequest(
                request.ApplicantId,
                request.ApplicantName,
                request.Age,
                request.CreditScore,
                request.MonthlyIncome,
                request.LoanAmount);

            if (!IsAgeEligible(loanRequest.Age))
            {
                reasons.Add($"Applicant must be at least {MinimumAge} years old.");
            }

            if (!IsCreditScoreEligible(loanRequest.CreditScore))
            {
                reasons.Add($"Credit score must be at least {MinimumCreditScore}.");
            }

            if (!IsIncomeEligible(loanRequest.MonthlyIncome))
            {
                reasons.Add($"Monthly income must be at least ${MinimumMonthlyIncome}.");
            }

            if (!IsLoanAmountEligible(loanRequest.MonthlyIncome, loanRequest.LoanAmount))
            {
                var maxLoanAmount = loanRequest.MonthlyIncome * LoanToIncomeMultiplier;
                reasons.Add($"Loan amount cannot exceed {LoanToIncomeMultiplier}x monthly income (Maximum: ${maxLoanAmount}).");
            }

            response.IsEligible = reasons.Count == 0;
            response.Reasons = reasons;
            response.Message = response.IsEligible
                ? "Loan application is eligible for processing."
                : "Loan application does not meet eligibility criteria.";

            return Task.FromResult(response);
        }

        private bool IsAgeEligible(int age) => age >= MinimumAge;

        private bool IsCreditScoreEligible(int creditScore) => creditScore >= MinimumCreditScore;

        private bool IsIncomeEligible(decimal monthlyIncome) => monthlyIncome >= MinimumMonthlyIncome;

        private bool IsLoanAmountEligible(decimal monthlyIncome, decimal loanAmount) 
            => loanAmount <= (monthlyIncome * LoanToIncomeMultiplier);
    }
}
