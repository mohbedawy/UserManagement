using MassTransit;
using System.Threading.Tasks;

namespace UserManagement.Consumers
{
    public abstract class GenericConsumer<T> : IConsumer<T> where T : class
    {
        public abstract Task Consume(ConsumeContext<T> context);
    }

    public abstract class GenericJobConsumer<T> : IJobConsumer<T> where T : class
    {
        public abstract Task Run(JobContext<T> context);
    }
}
