using MassTransit;
using MassTransit.JobService;
using System;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;
using UserManagement.Domain.EventJobContracts;

namespace UserManagement.Core
{
    public class UserJobSubmitter : IUserJobSubmitter
    {
        private readonly IJobService _jobService;
        private readonly IBus _bus;

        public UserJobSubmitter(IJobService jobService, IBus bus)
        {
            _jobService = jobService;
            _bus = bus;
        }

        public async Task<(Guid JobId, string TrackingNumber)> SubmitUserJobAsync(User user, string jobType)
        {
            var jobId = NewId.NextGuid();

            var job = new ProcessUserJob
            {
                UserId = user.Id,
                JobType = jobType,
                Payload = user
            };

            //var submissionContext = await _jobService.SubmitJobAsync<ProcessUserJob, User>(
            //    job,
            //    jobId: jobId,
            //    returnAddress: new Uri($"queue:{_bus.Address.Host}/job-results")
            //);

            //return (jobId, submissionContext.TrackingNumber);
            return (Guid.NewGuid(), "");
        }

        public async Task<string> GetJobStatusAsync(Guid jobId)
        {
            //var jobHandle = await _jobService.GetJobHandle<ProcessUserJob>(jobId);
            //if (jobHandle == null)
            //    return "Job not found";

            //var status = await jobHandle.GetStatusAsync();
            //return status.ToString();
            return "";
        }
    }
}