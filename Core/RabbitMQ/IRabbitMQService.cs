using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IRabbitMQService
    {
        public bool Post(string channel, object data);
    }
}
