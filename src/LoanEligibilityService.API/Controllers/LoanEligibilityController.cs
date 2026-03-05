using Microsoft.AspNetCore.Mvc;
using LoanEligibilityService.Application.DTOs;
using LoanEligibilityService.Application.Interfaces;

namespace LoanEligibilityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanEligibilityController : ControllerBase
    {
        private readonly ILoanEligibilityService _loanEligibilityService;
        private readonly ILogger<LoanEligibilityController> _logger;

        public LoanEligibilityController(
            ILoanEligibilityService loanEligibilityService,
            ILogger<LoanEligibilityController> logger)
        {
            _loanEligibilityService = loanEligibilityService;
            _logger = logger;
        }

        [HttpPost("check")]
        [ProducesResponseType(typeof(LoanEligibilityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoanEligibilityResponse>> CheckEligibility(
            [FromBody] LoanEligibilityRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(request.ApplicantId) || 
                string.IsNullOrWhiteSpace(request.ApplicantName))
            {
                return BadRequest("ApplicantId and ApplicantName are required.");
            }

            _logger.LogInformation(
                "Checking loan eligibility for applicant: {ApplicantId}", 
                request.ApplicantId);

            var response = await _loanEligibilityService.CheckEligibilityAsync(request);

            _logger.LogInformation(
                "Eligibility check result for {ApplicantId}: {IsEligible}", 
                request.ApplicantId, 
                response.IsEligible);

            return Ok(response);
        }
    }
}
