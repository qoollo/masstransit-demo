namespace ApiService.Models.Interfaces
{
    public interface IRoutingConfiguration
    {
        string? CartServiceAddress { get; set; }
        string? ApiServiceAddress { get; set; }
    }
}
