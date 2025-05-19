namespace UserManagement.API.Dependancies
{
    public class MassTransitSettings
    {
        public string Protocol { get; set; } = string.Empty;
        public string ClusterUrl { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<string> Nodes { get; set; } = new();
    }
}
