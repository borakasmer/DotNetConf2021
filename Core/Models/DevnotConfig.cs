using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DevnotConfig
    {

        #region Props
       
        public string PrivateKey { get; set; }
        public string RedisEndPoint { get; set; }
        public int RedisPort { get; set; }
        public string RabbitMqHostname { get; set; }
        public string RabbitMqUsername { get; set; }
        public string RabbitMqPassword { get; set; }
        public int RedisTimeout { get; set; }

        #endregion
        public DevnotConfig()
        {
            PrivateKey = "+IfWDqELQ2zBE6sI4D4ncSMBvTagujpBx0b5uieu8jI=æXB2DUtejEziVgiBPkRSahA==";
        }
    }
}
