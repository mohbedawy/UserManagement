using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserManagement.Domain.EventContracts;

namespace UserManagement.Consumers
{
    public class UserCreated_2_Consumer : GenericConsumer<UserCreatedEvent>
    {
        private readonly ILogger<UserCreated_2_Consumer> _logger;
        public UserCreated_2_Consumer(ILogger<UserCreated_2_Consumer> logger)
        {
            _logger = logger;
        }
        public override async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var user = context.Message.User;
            // Add your custom logic here
            _logger.LogInformation($"User created 2: {user.UserName} ({user.Email})");
        }
    }
}
