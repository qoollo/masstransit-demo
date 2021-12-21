using ApiService.Models.Interfaces;

namespace ApiService.Models.Implementations
{
    public class RoutingConfiguration : IRoutingConfiguration
    {
        public string? CartServiceAddress { get; set; }

        public string? ApiServiceAddress { get; set; }
    }
}
