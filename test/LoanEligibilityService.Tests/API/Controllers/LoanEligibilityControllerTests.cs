using FluentAssertions;
using LoanEligibilityService.API.Controllers;
using LoanEligibilityService.Application.DTOs;
using LoanEligibilityService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LoanEligibilityService.Tests.API.Controllers
{
    public class LoanEligibilityControllerTests
    {
        private readonly Mock<ILoanEligibilityService> _mockService;
        private readonly Mock<ILogger<LoanEligibilityController>> _mockLogger;
        private readonly LoanEligibilityController _sut;

        public LoanEligibilityControllerTests()
        {
            _mockService = new Mock<ILoanEligibilityService>();
            _mockLogger = new Mock<ILogger<LoanEligibilityController>>();
            _sut = new LoanEligibilityController(_mockService.Object, _mockLogger.Object);
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

        private static LoanEligibilityResponse CreateEligibleResponse() => new LoanEligibilityResponse
        {
            IsEligible = true,
            Reasons = new List<string>(),
            Message = "Loan application is eligible for processing."
        };

        [Fact]
        public async Task CheckEligibility_WhenRequestIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            LoanEligibilityRequest? request = null;

            // Act
            var result = await _sut.CheckEligibility(request!);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Request cannot be null.");
        }

        [Fact]
        public async Task CheckEligibility_WhenApplicantIdIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.ApplicantId = null!;

            // Act
            var result = await _sut.CheckEligibility(request);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("ApplicantId and ApplicantName are required.");
        }

        [Fact]
        public async Task CheckEligibility_WhenApplicantNameIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.ApplicantName = null!;

            // Act
            var result = await _sut.CheckEligibility(request);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("ApplicantId and ApplicantName are required.");
        }

        [Fact]
        public async Task CheckEligibility_WhenApplicantIdIsWhitespace_ShouldReturnBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.ApplicantId = "   ";

            // Act
            var result = await _sut.CheckEligibility(request);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("ApplicantId and ApplicantName are required.");
        }

        [Fact]
        public async Task CheckEligibility_WhenApplicantNameIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.ApplicantName = "";

            // Act
            var result = await _sut.CheckEligibility(request);

            // Assert
            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("ApplicantId and ApplicantName are required.");
        }

        [Fact]
        public async Task CheckEligibility_WhenValidRequestAndApplicantIsEligible_ShouldReturnOkWithResponse()
        {
            // Arrange
            var request = CreateValidRequest();
            var expectedResponse = CreateEligibleResponse();
            _mockService.Setup(s => s.CheckEligibilityAsync(request)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _sut.CheckEligibility(request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<LoanEligibilityResponse>().Subject;
            response.IsEligible.Should().BeTrue();
            response.Message.Should().Be("Loan application is eligible for processing.");
            _mockService.Verify(s => s.CheckEligibilityAsync(request), Times.Once);
        }

        [Fact]
        public async Task CheckEligibility_WhenValidRequestAndApplicantIsIneligible_ShouldReturnOkWithReasons()
        {
            // Arrange
            var request = CreateValidRequest();
            var expectedResponse = new LoanEligibilityResponse
            {
                IsEligible = false,
                Reasons = new List<string> { "Applicant must be at least 18 years old." },
                Message = "Loan application does not meet eligibility criteria."
            };
            _mockService.Setup(s => s.CheckEligibilityAsync(request)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _sut.CheckEligibility(request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<LoanEligibilityResponse>().Subject;
            response.IsEligible.Should().BeFalse();
            response.Reasons.Should().ContainSingle();
            response.Message.Should().Be("Loan application does not meet eligibility criteria.");
        }

        [Fact]
        public async Task CheckEligibility_WhenValidRequest_ShouldLogInformation()
        {
            // Arrange
            var request = CreateValidRequest();
            var expectedResponse = CreateEligibleResponse();
            _mockService.Setup(s => s.CheckEligibilityAsync(request)).ReturnsAsync(expectedResponse);

            // Act
            await _sut.CheckEligibility(request);

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("APP001")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeast(1));
        }
    }
}
