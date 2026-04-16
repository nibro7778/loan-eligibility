using FluentAssertions;
using LoanEligibilityService.Application.DTOs;
using Xunit;
using ServiceUnderTest = LoanEligibilityService.Application.Services.LoanEligibilityService;

namespace LoanEligibilityService.Tests.Application.Services
{
    public class LoanEligibilityServiceTests
    {
        private readonly ServiceUnderTest _sut;

        public LoanEligibilityServiceTests()
        {
            _sut = new ServiceUnderTest();
        }

        private static LoanEligibilityRequest CreateValidRequest() => new LoanEligibilityRequest
        {
            ApplicantId = "APP001",
            ApplicantName = "John Doe",
            Age = 30,
            CreditScore = 700,
            MonthlyIncome = 5000m,
            LoanAmount = 20000m
        };

        [Fact]
        public async Task CheckEligibilityAsync_WhenAllCriteriaMet_ShouldReturnEligible()
        {
            // Arrange
            var request = CreateValidRequest();

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
            result.Message.Should().Be("Loan application is eligible for processing.");
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenAgeBelowMinimum_ShouldReturnIneligibleWithReason()
        {
            // Arrange
            var request = CreateValidRequest();
            request.Age = 19;

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().ContainSingle(r => r.Contains("21 years old"));
            result.Message.Should().Be("Loan application does not meet eligibility criteria.");
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenAgeExactlyAtMinimum_ShouldReturnEligible()
        {
            // Arrange
            var request = CreateValidRequest();
            request.Age = 21;

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenCreditScoreBelowMinimum_ShouldReturnIneligibleWithReason()
        {
            // Arrange
            var request = CreateValidRequest();
            request.CreditScore = 599;

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().ContainSingle(r => r.Contains("600"));
            result.Message.Should().Be("Loan application does not meet eligibility criteria.");
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenCreditScoreExactlyAtMinimum_ShouldReturnEligible()
        {
            // Arrange
            var request = CreateValidRequest();
            request.CreditScore = 600;

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenMonthlyIncomeBelowMinimum_ShouldReturnIneligibleWithReason()
        {
            // Arrange
            var request = CreateValidRequest();
            request.MonthlyIncome = 1999m;
            request.LoanAmount = 5000m; // Keep loan within 10x of income

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain(r => r.Contains("2000"));
            result.Message.Should().Be("Loan application does not meet eligibility criteria.");
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenMonthlyIncomeExactlyAtMinimum_ShouldReturnEligible()
        {
            // Arrange
            var request = CreateValidRequest();
            request.MonthlyIncome = 2000m;
            request.LoanAmount = 5000m; // Within 10x of 2000

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenLoanAmountExceedsTenTimesIncome_ShouldReturnIneligibleWithReason()
        {
            // Arrange
            var request = CreateValidRequest();
            request.MonthlyIncome = 5000m;
            request.LoanAmount = 50001m; // Exceeds 10x income (50000)

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().Contain(r => r.Contains("10x monthly income"));
            result.Message.Should().Be("Loan application does not meet eligibility criteria.");
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenLoanAmountExactlyAtTenTimesIncome_ShouldReturnEligible()
        {
            // Arrange
            var request = CreateValidRequest();
            request.MonthlyIncome = 5000m;
            request.LoanAmount = 50000m; // Exactly 10x income

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Fact]
        public async Task CheckEligibilityAsync_WhenMultipleCriteriaFail_ShouldReturnMultipleReasons()
        {
            // Arrange
            var request = new LoanEligibilityRequest
            {
                ApplicantId = "APP002",
                ApplicantName = "Jane Doe",
                Age = 16,         // Below minimum
                CreditScore = 500, // Below minimum
                MonthlyIncome = 1000m, // Below minimum
                LoanAmount = 20000m    // Exceeds 10x income (10000)
            };

            // Act
            var result = await _sut.CheckEligibilityAsync(request);

            // Assert
            result.IsEligible.Should().BeFalse();
            result.Reasons.Should().HaveCount(4);
            result.Reasons.Should().Contain(r => r.Contains("21 years old"));
            result.Reasons.Should().Contain(r => r.Contains("600"));
            result.Reasons.Should().Contain(r => r.Contains("2000"));
            result.Reasons.Should().Contain(r => r.Contains("10x monthly income"));
            result.Message.Should().Be("Loan application does not meet eligibility criteria.");
        }
    }
}
