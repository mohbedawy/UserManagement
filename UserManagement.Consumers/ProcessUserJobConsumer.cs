using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserManagement.Domain.EventContracts;
using UserManagement.Domain.EventJobContracts;

namespace UserManagement.Consumers
{
    public class ProcessUserJobConsumer : GenericJobConsumer<ProcessUserJob>
    {
        private readonly ILogger<ProcessUserJobConsumer> _logger;
        
        public ProcessUserJobConsumer(ILogger<ProcessUserJobConsumer> logger)
        {
            _logger = logger;
        }

        public override async Task Run(JobContext<ProcessUserJob> context)
        {
            var job = context.Job;
            _logger.LogInformation($"Processing user job: {job.JobType} for user {job.UserId}");
            
            try
            {
                // Add your custom job processing logic here based on the JobType
                switch (job.JobType)
                {
                    case "UserRegistration":
                        await ProcessUserRegistration(job);
                        break;
                    case "UserVerification":
                        await ProcessUserVerification(job);
                        break;
                    case "UserProfileUpdate":
                        await ProcessUserProfileUpdate(job);
                        break;
                    default:
                        _logger.LogWarning($"Unknown job type: {job.JobType}");
                        break;
                }
                
                // If we get here without exceptions, the job completed successfully
                _logger.LogInformation($"Job {context.JobId} completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing job {context.JobId}: {ex.Message}");
                
                // Rethrowing the exception will cause MassTransit to mark the job as faulted
                // and potentially retry it based on your retry configuration
                throw;
            }
        }
        
        private async Task ProcessUserRegistration(ProcessUserJob job)
        {
            _logger.LogInformation($"Processing user registration for {job.Payload.UserName}");
            // Example: Send welcome email, initialize user resources, etc.
            await Task.Delay(2000); // Simulating work
        }

        private async Task ProcessUserVerification(ProcessUserJob job)
        {
            _logger.LogInformation($"Processing user verification for {job.Payload.UserName}");
            // Example: Verify user credentials, validate email, etc.
            await Task.Delay(1500); // Simulating work
        }

        private async Task ProcessUserProfileUpdate(ProcessUserJob job)
        {
            _logger.LogInformation($"Processing profile update for {job.Payload.UserName}");
            // Example: Update user profile in external systems
            await Task.Delay(1000); // Simulating work
        }
    }
}
