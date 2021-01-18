using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevnotProduct.Controllers
{
    public interface IHubProductDispatcher
    {
        Task PushProduct(ViewProduct product);
    }
}
