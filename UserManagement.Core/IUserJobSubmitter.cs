using System;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;
using UserManagement.Domain.EventJobContracts;

namespace UserManagement.Core
{
    public interface IUserJobSubmitter
    {
        /// <summary>
        /// Submit a user job for background processing.
        /// </summary>
        /// <param name="user">The user data for processing</param>
        /// <param name="jobType">Type of job to perform</param>
        /// <returns>A tuple containing the job ID (guid) and tracking number (string)</returns>
        Task<(Guid JobId, string TrackingNumber)> SubmitUserJobAsync(User user, string jobType);
        
        /// <summary>
        /// Gets the status of a user job.
        /// </summary>
        /// <param name="jobId">The job identifier</param>
        /// <returns>The status of the job</returns>
        Task<string> GetJobStatusAsync(Guid jobId);
    }
}