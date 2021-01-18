using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.PartialEntites
{

    [AttributeUsage(AttributeTargets.All)]
    public class CryptoData : System.Attribute
    {
        public CryptoData()
        {
        }
        public int id { get; set; }
    }
}

