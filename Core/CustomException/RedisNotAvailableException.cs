using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CustomException
{
    public class RedisNotAvailableException : Exception
    {
        public string _errorCode = "431";
        public override string Message
        {
            get
            {
                return "Redis is not available.";
            }
        }

        public string ErrorCode
        {
            get
            {
                return _errorCode;
            }
        }
    }
}
