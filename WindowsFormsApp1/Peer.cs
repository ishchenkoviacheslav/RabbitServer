using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService
{
    class ClientPeer
    {
        public string UserName { get; set; }
        public string QeueuName { get; set; }
        public DateTime LastUpTime { get; set; }
    }
}
