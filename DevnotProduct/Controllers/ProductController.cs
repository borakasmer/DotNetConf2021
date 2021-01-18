using AutoMapper;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace DevnotProduct.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IHubProductDispatcher _dispatcher;
        private readonly IMapper _mapper;
        public ProductController(IProductService productServic, IHubProductDispatcher dispatcher, IMapper mapper)
        {
            _productService = productServic;
            _dispatcher = dispatcher;
            _mapper = mapper;
        }

        [HttpGet("{Id}")]
        public ViewProduct GetById(int Id)
        {
            return _productService.GetProductById(Id);
        }

        [HttpGet("GetExchangeList")]
        public IEnumerable<ViewExchange> GetExchangeList()
        {
            return _productService.GetExchangeList();
        }

        [HttpGet("GetProductList")]
        public IEnumerable<ViewProduct> GetProductList()
        {
            return _productService.GetProductList();
        }

        [HttpPost("InsertProduct")]
        public IEnumerable<ViewProduct> InsertProduct([FromBody] ViewProduct model)
        {
            if (model != null)
            {
                if (model.ID > 0)
                    return _productService.UpdateProduct(model);
                else
                    return _productService.InsertProduct(model);
            }
            return new List<ViewProduct>();
        }
        [HttpPost("PushProduct")]
        public async Task<ActionResult> PushProduct([FromBody] ExchangeQueue productQueue)
        {
            try
            {
                System.Threading.Thread.Sleep(2000); //:) Hahahaha

                ViewProduct product = _mapper.Map<ViewProduct>(productQueue);
                product = _productService.UpdateProduct(product).FirstOrDefault(p => p.ID == product.ID);
                await _dispatcher.PushProduct(product);                
            }
            catch (System.Exception ex)
            {
                int i = 0;
            }
            return Ok();
        }
    }
    public class ProductHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("GetConnectionId", this.Context.ConnectionId);
        }
    }
}
