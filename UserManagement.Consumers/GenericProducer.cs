using MassTransit;
using MassTransit.Context;
using System;
using System.Threading.Tasks;
using UserManagement.Domain.EventContracts;
using UserManagement.Infrastructure.Database;

namespace UserManagement.Consumers
{
    public class GenericProducer
    {
        private readonly IBus _bus;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly UserManagementDbContext _dbContext;

        public GenericProducer(IBus bus, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint, UserManagementDbContext managementDbContext, UserManagementDbContext dbContext)
        {
            _bus = bus;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
            this._dbContext = managementDbContext;
        }

        public async Task PublishAsync<T>(T message, string routingKey = default) where T : class
        {
            Console.WriteLine($"Publishing message of type {typeof(T).Name} to routingKey: {routingKey}");
            Guid guid = Guid.NewGuid();

           
            await _publishEndpoint.Publish(message, context =>
            {
                //if (context is RabbitMqSendContext<T> config)
                //{
                //    // You can access RabbitMQ-specific properties here if needed
                //    // For example: config.DeliveryMode = 2; // persistent
                //}
                context.CorrelationId = guid;
                context.ConversationId = guid;
                context.InitiatorId = guid;
                context.RequestId = guid;
                context.Headers.Set("MessageType", typeof(T).Name);
                context.Headers.Set("MessageId", guid);
                context.Headers.Set("OutboxId", guid); // Set OutboxId header

                if (!string.IsNullOrWhiteSpace(routingKey))
                {
                    context.SetRoutingKey(routingKey);
                }
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task SendAsync<T>(T message, string queueKey) where T : class
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueKey}"));
            await sendEndpoint.Send<T>(message);
            await _dbContext.SaveChangesAsync();
        }
    }
}
