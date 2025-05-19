using MassTransit;
using UserManagement.Consumers;
using Microsoft.Extensions.Logging;
using UserManagement.Domain.EventJobContracts;
using MassTransit.JobService;
using UserManagement.Domain.EventContracts;
using RabbitMQ.Client;
using UserManagement.Infrastructure.Database;
namespace UserManagement.API.Dependancies
{
    public static class ConfigureMiddlewares
    {

        public static void ConfigureMassTransit(this WebApplicationBuilder builder)
        {
            var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();
            var massTransitSettings = builder.Configuration.GetSection("MassTransit").Get<MassTransitSettings>();

            builder.Services.AddMassTransit(x =>
            {
                // add outbox entityFrmework
                x.AddEntityFrameworkOutbox<UserManagementDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                    o.QueryDelay = TimeSpan.FromSeconds(1); // Ensure a query delay is set for better debugging
                    //o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);
                    //o.DisableInboxCleanupService();
                    // Add logging
                
                });

                x.AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug().SetMinimumLevel(LogLevel.Warning);
                });
                x.AddConsumer<UserCreatedConsumer>();
                x.AddConsumer<UserCreated_2_Consumer>();
                x.AddConsumer<UserUpdatedConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    // set Job Consumer
                    cfg.Host(new Uri($"{massTransitSettings.Protocol}://{massTransitSettings.ClusterUrl}"),
                                        hostConfig =>
                                        {
                                            hostConfig.Username(massTransitSettings.UserName);
                                            hostConfig.Password(massTransitSettings.Password);
                                            if (massTransitSettings.Nodes != null && massTransitSettings.Nodes.Any())
                                                hostConfig.UseCluster(e =>
                                                {
                                                    foreach (var node in massTransitSettings.Nodes)
                                                    {
                                                        logger.LogInformation($"Configuring rabbitMq node {node}.");
                                                        e.Node(node);
                                                    }
                                                });
                                        });


                    cfg.MessageTopology.SetEntityNameFormatter(new CustomMessageNameFormatter());

                    cfg.Publish<UserCreatedEvent>(pps =>
                            pps.ExchangeType = ExchangeType.Topic);

                    // set auto routing
                    cfg.Send<UserCreatedEvent>(sendCfg =>
                           sendCfg.UseRoutingKeyFormatter(ctx => "User.Created"));

                    cfg.Publish<UserUpdatedEvent>(pps =>
                           pps.ExchangeType = ExchangeType.Topic);
                    cfg.Send<UserUpdatedEvent>(sendCfg =>
                            sendCfg.UseRoutingKeyFormatter(ctx => "User.Updated"));

                    #region Configuring normal Consumers

                    // configuring Consumers
                    cfg.ReceiveEndpoint("UserCreatedConsumer_Queue", e =>
                    {
                        e.ConfigureConsumeTopology = false;
                        e.Bind<UserCreatedEvent>(bind =>
                        {
                            bind.RoutingKey = "User.Created";
                            bind.Durable = true;
                            bind.AutoDelete = false;
                            bind.ExchangeType = ExchangeType.Topic;
                        });

                        e.ConfigureConsumer<UserCreatedConsumer>(context);
                        e.UseEntityFrameworkOutbox<UserManagementDbContext>(context);
                    });

                    cfg.ReceiveEndpoint("UserCreatedConsumer_2_Queue", e =>
                    {
                        e.ConfigureConsumeTopology = false;
                        e.Bind<UserCreatedEvent>(bind =>
                        {
                            bind.RoutingKey = "User.Created2";
                            bind.Durable = true;
                            bind.AutoDelete = false;
                            bind.ExchangeType = ExchangeType.Topic;
                        });

                        e.ConfigureConsumer<UserCreated_2_Consumer>(context);
                        e.UseEntityFrameworkOutbox<UserManagementDbContext>(context);
                    });

                    cfg.ReceiveEndpoint("UserUpdatedConsumer_Queue", e =>
                    {
                        e.ConfigureConsumeTopology = false;
                        e.Bind<UserCreatedEvent>(bind =>
                        {
                            bind.Durable = true;
                            bind.AutoDelete = false;
                            bind.ExchangeType = ExchangeType.Topic;
                        });

                        e.ConfigureConsumer<UserUpdatedConsumer>(context);
                        e.UseEntityFrameworkOutbox<UserManagementDbContext>(context);
                    });

                    #endregion
                });
            });
        }
    }
}
