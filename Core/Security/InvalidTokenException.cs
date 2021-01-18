using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class InvalidTokenException : Exception
    {
        public string _errorCode = "430";
        public override string Message
        {
            get
            {
                return "Invalid Token Exeception";
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
