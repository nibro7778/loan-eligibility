using LoanEligibilityService.Domain.Models;
using Xunit;

namespace LoanEligibilityService.Tests.Domain
{
    public class LoanRequestTests
    {
        [Fact]
        public void LoanRequest_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var applicantId = "APP001";
            var applicantName = "John Doe";
            var age = 30;
            var creditScore = 700;
            var monthlyIncome = 5000m;
            var loanAmount = 40000m;

            // Act
            var loanRequest = new LoanRequest(
                applicantId,
                applicantName,
                age,
                creditScore,
                monthlyIncome,
                loanAmount);

            // Assert
            Assert.Equal(applicantId, loanRequest.ApplicantId);
            Assert.Equal(applicantName, loanRequest.ApplicantName);
            Assert.Equal(age, loanRequest.Age);
            Assert.Equal(creditScore, loanRequest.CreditScore);
            Assert.Equal(monthlyIncome, loanRequest.MonthlyIncome);
            Assert.Equal(loanAmount, loanRequest.LoanAmount);
        }

        [Theory]
        [InlineData("APP001", "John Doe", 25, 650, 5000, 30000)]
        [InlineData("APP002", "Jane Smith", 35, 720, 8000, 50000)]
        [InlineData("APP003", "Bob Jones", 45, 800, 10000, 100000)]
        public void LoanRequest_Constructor_HandlesVariousInputs(
            string applicantId,
            string applicantName,
            int age,
            int creditScore,
            decimal monthlyIncome,
            decimal loanAmount)
        {
            // Act
            var loanRequest = new LoanRequest(
                applicantId,
                applicantName,
                age,
                creditScore,
                monthlyIncome,
                loanAmount);

            // Assert
            Assert.NotNull(loanRequest);
            Assert.Equal(applicantId, loanRequest.ApplicantId);
            Assert.Equal(applicantName, loanRequest.ApplicantName);
        }
    }
}
