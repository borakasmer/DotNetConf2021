using Core.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevnotProduct.Controllers
{
    public class HubProductDispatcher:IHubProductDispatcher
    {
        private readonly IHubContext<ProductHub> _hubContext;
        public HubProductDispatcher(IHubContext<ProductHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushProduct(ViewProduct product)
        {
            await this._hubContext.Clients.All.SendAsync("PushProduct", product, product.ConnectionId);
        }
    }
}
