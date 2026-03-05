using LoanEligibilityService.Application.DTOs;
using LoanEligibilityService.Application.Services;
using Xunit;

namespace LoanEligibilityService.Tests.Application
{
    public class LoanEligibilityServiceTests
    {
        private readonly LoanEligibilityService.Application.Services.LoanEligibilityService _service;

        public LoanEligibilityServiceTests()
        {
            _service = new LoanEligibilityService.Application.Services.LoanEligibilityService();
        }

        [Fact]
        public async Task CheckEligibility_AllCriteriaMet_ReturnsEligible()
        {
            // Arrange
            var request = new LoanEligibilityRequest
            {
                ApplicantId = "TEST001",
                ApplicantName = "John Doe",
                Age = 30,
                CreditScore = 700,
                MonthlyIncome = 5000,
                LoanAmount = 40000
            };

            // Act
            var result = await _service.CheckEligibilityAsync(request);

            // Assert
            Assert.True(result.IsEligible);
            Assert.Empty(result.Reasons);
            Assert.Equal("Loan application is eligible for processing.", result.Message);
        }

        [Fact]
        public async Task CheckEligibility_AgeBelowMinimum_ReturnsNotEligible()
        {
            // Arrange
            var request = new LoanEligibilityRequest
            {
                ApplicantId = "TEST002",
                ApplicantName = "Jane Smith",
                Age = 17,
                CreditScore = 700,
                MonthlyIncome = 5000,
                LoanAmount = 40000
            };

            // Act
            var result = await _service.CheckEligibilityAsync(request);

            // Assert
            Assert.False(result.IsEligible);
            Assert.Contains("Applicant must be at least 18 years old.", result.Reasons);
        }

        [Fact]
        public async Task CheckEligibility_CreditScoreBelowMinimum_ReturnsNotEligible()
        {
            // Arrange
            var request = new LoanEligibilityRequest
            {
                ApplicantId = "TEST003",
                ApplicantName = "Bob Jones",
                Age = 30,
                CreditScore = 550,
                MonthlyIncome = 5000,
                LoanAmount = 40000
            };

            // Act
            var result = await _service.CheckEligibilityAsync(request);

            // Assert
            Assert.False(result.IsEligible);
            Assert.Contains("Credit score must be at least 600.", result.Reasons);
        }

        [Fact]
        public async Task CheckEligibility_IncomeBelowMinimum_ReturnsNotEligible()
        {
            // Arrange
            var request = new LoanEligibilityRequest
            {
                ApplicantId = "TEST004",
                ApplicantName = "Alice Brown",
                Age = 30,
                CreditScore = 700,
                MonthlyIncome = 1500,
                LoanAmount = 10000
            };

            // Act
            var result = await _service.CheckEligibilityAsync(request);

            // Assert
            Assert.False(result.IsEligible);
            Assert.Contains("Monthly income must be at least $2000.", result.Reasons);
        }

        [Fact]
        public async Task CheckEligibility_LoanAmountExceedsLimit_ReturnsNotEligible()
        {
            // Arrange
            var request = new LoanEligibilityRequest
            {
                ApplicantId = "TEST005",
                ApplicantName = "Charlie Wilson",
                Age = 30,
                CreditScore = 700,
                MonthlyIncome = 5000,
                LoanAmount = 60000 // Exceeds 10x monthly income
            };

            // Act
            var result = await _service.CheckEligibilityAsync(request);

            // Assert
            Assert.False(result.IsEligible);
            Assert.Contains("Loan amount cannot exceed 10x monthly income", result.Reasons[0]);
        }

        [Fact]
        public async Task CheckEligibility_MultipleCriteriaFail_ReturnsAllReasons()
        {
            // Arrange
            var request = new LoanEligibilityRequest
            {
                ApplicantId = "TEST006",
                ApplicantName = "David Miller",
                Age = 17,
                CreditScore = 550,
                MonthlyIncome = 1500,
                LoanAmount = 30000
            };

            // Act
            var result = await _service.CheckEligibilityAsync(request);

            // Assert
            Assert.False(result.IsEligible);
            Assert.Equal(4, result.Reasons.Count);
            Assert.Contains(result.Reasons, r => r.Contains("at least 18 years old"));
            Assert.Contains(result.Reasons, r => r.Contains("Credit score"));
            Assert.Contains(result.Reasons, r => r.Contains("Monthly income"));
            Assert.Contains(result.Reasons, r => r.Contains("Loan amount"));
        }

        [Theory]
        [InlineData(18, 600, 2000, 20000, true)]
        [InlineData(25, 650, 3000, 30000, true)]
        [InlineData(40, 750, 10000, 100000, true)]
        [InlineData(17, 600, 2000, 20000, false)]
        [InlineData(18, 599, 2000, 20000, false)]
        [InlineData(18, 600, 1999, 20000, false)]
        [InlineData(18, 600, 2000, 20001, false)]
        public async Task CheckEligibility_VariousScenarios_ReturnsExpectedResult(
            int age, int creditScore, decimal monthlyIncome, decimal loanAmount, bool expectedEligible)
        {
            // Arrange
            var request = new LoanEligibilityRequest
            {
                ApplicantId = $"TEST{age}{creditScore}",
                ApplicantName = "Test User",
                Age = age,
                CreditScore = creditScore,
                MonthlyIncome = monthlyIncome,
                LoanAmount = loanAmount
            };

            // Act
            var result = await _service.CheckEligibilityAsync(request);

            // Assert
            Assert.Equal(expectedEligible, result.IsEligible);
        }
    }
}
