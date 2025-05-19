using MassTransit;
using MassTransit.Transports;

namespace UserManagement.Consumers
{
    public class CustomMessageNameFormatter : IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            var typeName = typeof(T).FullName;
            var parts = typeName.Split('.');
            var namespaceRoot = parts[0]; 
            var entityName = parts[^1]; 
            return $"{namespaceRoot}:{entityName}";
        }
    }
}
