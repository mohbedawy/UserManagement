using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.DTO;
using UserManagement.Domain.Entities;

namespace UserManagement.Domain.EventJobContracts
{
    public class ProcessUserJob
    {
        public Guid UserId { get; set; }
        public string JobType { get; set; }
        public User Payload { get; set; }
    }
}
