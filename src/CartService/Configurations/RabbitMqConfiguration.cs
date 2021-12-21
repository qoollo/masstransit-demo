using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Configurations
{
    public class RabbitMqConfiguration
    {
        public string? Hostname { get; set; }
        public string? VirtualHost { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
