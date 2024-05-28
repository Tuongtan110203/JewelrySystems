using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000")]

    public class OrdersController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IOrderRepository orderRepository;

        public OrdersController(JewelrySalesSystemDbContext dbContext,IMapper mapper,IOrderRepository orderRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.orderRepository = orderRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orderDomain = await orderRepository.GetAllOrders();
            return Ok(mapper.Map<List<OrdersDTO>>(orderDomain));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var orderDomain = await orderRepository.GetOrdersById(id);
            if (orderDomain == null) NotFound();
            return Ok(mapper.Map<OrdersDTO>(orderDomain));

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDTO updateOrderDTO)
        {
            var existing = await orderRepository.GetOrdersById(id);
            if (existing == null)
            {
                return NotFound();
            }

            var goldTypeDomainModel = mapper.Map<Orders>(updateOrderDTO);
            await orderRepository.UpdateGOrders(id, goldTypeDomainModel);

            return Ok(mapper.Map<OrdersDTO>(goldTypeDomainModel));
        }
        [HttpPost]
        public async Task<ActionResult<GoldType>> CreateOrder(AddOrderDTO addOrderDTO)
        {
            var orderDomain = mapper.Map<Orders>(addOrderDTO);
            if (orderDomain == null) { return NotFound(); }
            orderDomain = await orderRepository.AddOrders(orderDomain);
            return Ok(mapper.Map<OrdersDTO>(orderDomain));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var checkExist = await orderRepository.DeleteOrders(id);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<OrdersDTO>(checkExist));
        }

        [HttpGet("GetNumberOfOrders")]
        public async Task<IActionResult> GetNumberOfOrders()
        {
            var orderDomain = await orderRepository.GetNumberOfOrdersAsync();
            return Ok(orderDomain);
        }

        [HttpGet("GetWaitingOrders")]
        public async Task<IActionResult> GetWaitingOrders()
        {
            var orderDomain = await orderRepository.GetWaitingOrdersAsync();
            return Ok(orderDomain);
        }

        [HttpGet("GetPaidOrders")]
        public async Task<IActionResult> GetPaidOrders()
        {
            var orderDomain = await orderRepository.GetPaidOrdersAsync();
            return Ok(orderDomain);
        }

        [HttpGet("GetDoneOrders")]
        public async Task<IActionResult> GetDoneOrdersAsync()
        {
            var orderDomain = await orderRepository.GetDoneOrdersAsync();
            return Ok(orderDomain);
        }
        [HttpGet("GetNumberOfOrderDetails/{id}")]
        public async Task<IActionResult> GetNumberOfOrderDetails(int id)
        {
            var orderDomain = await orderRepository.GetNumberOfOrderDetailsAsync(id);
            return Ok(orderDomain);
        }
    }
}
