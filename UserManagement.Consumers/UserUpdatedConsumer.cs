using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;
using UserManagement.Domain.EventContracts;
using UserManagement.Infrastructure.Database;

namespace UserManagement.Consumers
{
    public class UserUpdatedConsumer : GenericConsumer<UserUpdatedEvent>
    {
        private readonly ILogger<UserCreatedConsumer> _logger;
        private readonly GenericProducer producer;


        public UserUpdatedConsumer(ILogger<UserCreatedConsumer> logger, GenericProducer producer)
        {
            this._logger = logger;
            this.producer = producer;
        }
        public override async Task Consume(ConsumeContext<UserUpdatedEvent> context)
        {
            try
            {
                // throw new Exception();
                var user = context.Message.User;
                // send to consumer 2

                await producer.PublishAsync(new UserCreatedEvent
                {
                    User = new User
                    {
                        Id = user.Id,
                        UserName = "Mohamed"+user.UserName,
                        Email = "Mohamed"+user.Email,
                        FirstName = "Mohamed" + user.FirstName,
                        LastName = "Mohamed" + user.LastName,
                    }
                }, "User.Created2");


                _logger.LogInformation($"User created: {user.UserName} ({user.Email})");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
