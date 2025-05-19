using MassTransit;
using UserManagement.Domain.Entities;

namespace UserManagement.Domain.EventContracts
{
    public class UserCreatedEvent
    {
        public User User { get; set; }
    }
    public class UserUpdatedEvent
    {
        public User User { get; set; }
    }
    public class UserDeletedEvent
    {
        public Guid UserId { get; set; }
    }
}
