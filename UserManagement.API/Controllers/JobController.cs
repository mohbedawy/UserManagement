using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserManagement.Core;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.IRepository;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)] // Hides the controller from API documentation
    public class JobController : ControllerBase
    {
        private readonly IUserJobSubmitter _userJobSubmitter;
        private readonly IUserRepository _userRepository;

        public JobController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("submit/{userId}")]
        public async Task<IActionResult> SubmitJob(Guid userId, [FromQuery] string jobType)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                return NotFound($"User with ID {userId} not found.");

            var (jobId, trackingNumber) = await _userJobSubmitter.SubmitUserJobAsync(user, jobType);

            return Ok(new
            {
                JobId = jobId,
                TrackingNumber = trackingNumber,
                UserId = userId,
                JobType = jobType,
                SubmittedAt = DateTime.UtcNow
            });
        }

        [HttpGet("status/{jobId}")]
        public async Task<IActionResult> GetJobStatus(Guid jobId)
        {
            var status = await _userJobSubmitter.GetJobStatusAsync(jobId);

            return Ok(new
            {
                JobId = jobId,
                Status = status,
                CheckedAt = DateTime.UtcNow
            });
        }

        [HttpPost("test")]
        public async Task<IActionResult> SubmitTestJob([FromQuery] string jobType = "UserRegistration")
        {
            var testUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser_" + Guid.NewGuid().ToString().Substring(0, 8),
                Email = $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com",
                FirstName = "Test",
                LastName = "User",
            };

            var (jobId, trackingNumber) = await _userJobSubmitter.SubmitUserJobAsync(testUser, jobType);

            return Ok(new
            {
                JobId = jobId,
                TrackingNumber = trackingNumber,
                TestUser = testUser,
                JobType = jobType,
                SubmittedAt = DateTime.UtcNow,
                Message = "Test job submitted successfully"
            });
        }
    }
}