using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[AllowAnonymous]
    public class OrdersController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IOrderRepository orderRepository;
        private readonly IUsersRepository usersRepository;

        public OrdersController(JewelrySalesSystemDbContext dbContext, IMapper mapper, IOrderRepository orderRepository, IUsersRepository usersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.orderRepository = orderRepository;
            this.usersRepository = usersRepository;
        }
        [HttpGet]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var orderDomain = await orderRepository.GetAllOrders(pageNumber, pageSize);
            return Ok(mapper.Map<List<OrdersDTO>>(orderDomain));
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var orderDomain = await orderRepository.GetOrdersById(id);
            if (orderDomain == null) return NotFound();

            // Map domain model to DTO
            var orderDTO = mapper.Map<OrdersDTO>(orderDomain);

            // Retrieve and map user information for SaleById, CashierId, and ServicerId
            if (!string.IsNullOrEmpty(orderDomain.SaleById))
            {
                orderDTO.sale = await usersRepository.GetUserByUserNameAsync(orderDomain.SaleById);
            }
            if (!string.IsNullOrEmpty(orderDomain.CashierId))
            {
                orderDTO.cashier = await usersRepository.GetUserByUserNameAsync(orderDomain.CashierId);
            }
            if (!string.IsNullOrEmpty(orderDomain.ServicerId))
            {
                orderDTO.services = await usersRepository.GetUserByUserNameAsync(orderDomain.ServicerId);
            }

            return Ok(orderDTO);
        }


        [HttpGet("GetOrderDetails/{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetOrderDetailsAsync(int id)
        {
            var orderDetails = await orderRepository.GetOrderDetailsAsync(id);
            if (orderDetails == null || !orderDetails.Any()) return NotFound();

            var orderDetailsDTO = mapper.Map<List<OrderDetailsDTO>>(orderDetails);

            var order = orderDetails.FirstOrDefault()?.Orders;
            if (order != null)
            {
                var saleUser = await usersRepository.GetUserByUserNameAsync(order.SaleById);
                var cashierUser = await usersRepository.GetUserByUserNameAsync(order.CashierId);
                var servicesUser = await usersRepository.GetUserByUserNameAsync(order.ServicerId);

                foreach (var detail in orderDetailsDTO)
                {
                    detail.Sale = saleUser;
                    detail.Cashier = cashierUser;
                    detail.Services = servicesUser;
                }
            }

            return Ok(orderDetailsDTO);
        }


        [HttpGet("getOrderByOrderCode")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetOrderByOrderCode(string code)
        {
            var orderDomain = await orderRepository.GetOrdersByOrderCode(code);
            if (orderDomain == null) return NotFound();
            var orderDTO = mapper.Map<List<OrderDetailsDTO>>(orderDomain);
            return Ok(orderDTO);

        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
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

        //[HttpPost]
        //[Authorize(Roles = "Staff,Manager")]
        //public async Task<ActionResult<GoldType>> CreateOrder([FromForm] AddOrderDTO addOrderDTO)
        //{
        //    var orderDomain = mapper.Map<Orders>(addOrderDTO);
        //    if (orderDomain == null) { return NotFound(); }
        //    orderDomain = await orderRepository.AddOrders(orderDomain);
        //    return Ok(mapper.Map<OrdersDTO>(orderDomain));
        //}


        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
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
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetNumberOfOrders()
        {
            var orderDomain = await orderRepository.GetNumberOfOrdersAsync();
            return Ok(orderDomain);
        }

        //[HttpGet("GetWaitingOrders")]
        //[Authorize(Roles = "Staff,Manager")]
        //public async Task<IActionResult> GetWaitingOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        //{
        //    var orderDomain = await orderRepository.GetWaitOrdersAsync(pageNumber, pageSize);
        //    var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
        //    return Ok(orderDTO);
        //}

        /*[HttpGet("GetPaidOrders")]
                   [Authorize(Roles = "Staff,Manager")]
                   public async Task<IActionResult> GetPaidOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
                   {
                       var orderDomain = await orderRepository.GetPaidOrdersAsync(pageNumber, pageSize);
                       var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                       return Ok(orderDTO);
                   }

                   [HttpGet("GetDoneOrders")]
                   [Authorize(Roles = "Staff,Manager")]
                   public async Task<IActionResult> GetDoneOrdersAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
                   {
                       var orderDomain = await orderRepository.GetDoneOrdersAsync(pageNumber, pageSize);
                       var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                       return Ok(orderDTO);
                   }*/


        [HttpGet("GetNumberOfOrderDetails/{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetNumberOfOrderDetails(int id)
        {
            var numberOfOrderDetails = await orderRepository.GetNumberOfOrderDetailsAsync(id);
            //  var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
            return Ok(numberOfOrderDetails);
        }

        [HttpGet("get-number-of-order-quantity/{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetNumberOfOrderQuantity(int id)
        {
            var numberOfQuantity = await orderRepository.GetNumberOfOrderQuantitAsync(id);
            //  var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
            return Ok(numberOfQuantity);
        }

        [HttpGet("get-status-order/{option}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetStatusOrder(string option, [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 1000,
              [FromQuery] string? sortBy = null, [FromQuery] bool isAscending = true)
        {
            if (option == "paid")
            {
                var orderDomain = await orderRepository.GetPaidOrdersAsync(pageNumber, pageSize, sortBy, isAscending);

                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "done")
            {
                var orderDomain = await orderRepository.GetDoneOrdersAsync(pageNumber, pageSize, sortBy, isAscending);
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "waiting")
            {
                var orderDomain = await orderRepository.GetWaitingOrdersAsync(pageNumber, pageSize, sortBy, isAscending);
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "cancel")
            {
                var orderDomain = await orderRepository.GetCancelOrdersAsync(pageNumber, pageSize, sortBy, isAscending);
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "expire")
            {
                var orderDomain = await orderRepository.GetExpiredOrdersAsync(pageNumber, pageSize, sortBy, isAscending);
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "paying")
            {
                var orderDomain = await orderRepository.GetPayingOrdersAsync(pageNumber, pageSize, sortBy, isAscending);
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "wait")
            {
                var orderDomain = await orderRepository.GetWaitOrdersAsync(pageNumber, pageSize, sortBy, isAscending);
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "all")
            {
                var orderDomain = await orderRepository.GetStatusOrdersAsync(pageNumber, pageSize, sortBy, isAscending);
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }
        }


        [HttpGet("get-orders/{option}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetOrders(string option)
        {
            if (option == "today")
            {
                var orderDomain = await orderRepository.GetTodayOrdersAsync();
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "this-week")
            {
                var orderDomain = await orderRepository.GetThisWeekOrdersAsync();
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "this-month")
            {
                var orderDomain = await orderRepository.GetThisMonthOrdersAsync();
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else if (option == "this-year")
            {
                var orderDomain = await orderRepository.GetThisYearOrdersAsync();
                var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
                return Ok(orderDTO);
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }
        }

        /*  [HttpGet("GetThisWeekOrders")]
          public async Task<IActionResult> GetThisWeekOrders()
          {
              var orderDomain = await orderRepository.GetThisWeekOrdersAsync();
              var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
              return Ok(orderDTO);
          }

          [HttpGet("GetThisMonthOrders")]
          public async Task<IActionResult> GetThisMonthOrders()
          {
              var orderDomain = await orderRepository.GetThisMonthOrdersAsync();
              var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
              return Ok(orderDTO);
          }

          [HttpGet("GetThisYearOrders")]
          public async Task<IActionResult> GetThisYearOrders()
          {
              var orderDomain = await orderRepository.GetThisYearOrdersAsync();
              var orderDTO = mapper.Map<List<OrdersDTO>>(orderDomain);
              return Ok(orderDTO);
          }*/

        /*  [HttpGet("GetNumberOfByCashOrders")]
          public async Task<IActionResult> GetNumberOfByCashOrders()
          {
              var num = await orderRepository.GetNumberOfByCashOrdersAsync();
              return Ok(num);
          }

          [HttpGet("GetNumberOfByBankTransferOrders")]
          public async Task<IActionResult> GetNumberOfByBankTransferOrders()
          {
              var num = await orderRepository.GetNumberOfByBankTransferOrdersAsync();
              return Ok(num);
          }*/

        [HttpGet("get-number-of-orders-by-payment-type/{option}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetNumberOfOrdersByPaymentType(string option)
        {
            if (option == "bank-transfer")
            {
                var num = await orderRepository.GetNumberOfByBankTransferOrdersAsync();
                return Ok(num);
            }
            else if (option == "cash")
            {
                var num = await orderRepository.GetNumberOfByCashOrdersAsync();
                return Ok(num);
            }
            else if (option == "both")
            {
                var num = await orderRepository.GetNumberOfByCashAndBankTransferOrdersAsync();
                return Ok(num);
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }

        }

        [HttpGet("get-number-and-total-payment")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetNumberAndTotalPayment()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);

            // all
            var numBankTransferAll = await orderRepository.GetNumberOfByBankTransferOrdersAsync();
            var numCashAll = await orderRepository.GetNumberOfByCashOrdersAsync();

            var totalPriceBankTransferAll = await orderRepository.GetTotalPriceOfBankTransferAsync();
            var totalPriceByCashAll = await orderRepository.GetTotalPriceOfByCashAsync();

            // today
            var numBankTransferToday = await orderRepository.GetNumberOfByBankTransferOrdersAsync(today, today.AddDays(1));
            var numCashToday = await orderRepository.GetNumberOfByCashOrdersAsync(today, today.AddDays(1));

            var totalPriceBankTransferToday = await orderRepository.GetTotalPriceOfBankTransferAsync(today, today.AddDays(1));
            var totalPriceByCashToday = await orderRepository.GetTotalPriceOfByCashAsync(today, today.AddDays(1));

            // this week
            var numBankTransferThisWeek = await orderRepository.GetNumberOfByBankTransferOrdersAsync(startOfWeek, today.AddDays(1));
            var numCashThisWeek = await orderRepository.GetNumberOfByCashOrdersAsync(startOfWeek, today.AddDays(1));

            var totalPriceBankTransferThisWeek = await orderRepository.GetTotalPriceOfBankTransferAsync(startOfWeek, today.AddDays(1));
            var totalPriceByCashThisWeek = await orderRepository.GetTotalPriceOfByCashAsync(startOfWeek, today.AddDays(1));

            // this month
            var numBankTransferThisMonth = await orderRepository.GetNumberOfByBankTransferOrdersAsync(startOfMonth, today.AddDays(1));
            var numCashThisMonth = await orderRepository.GetNumberOfByCashOrdersAsync(startOfMonth, today.AddDays(1));

            var totalPriceBankTransferThisMonth = await orderRepository.GetTotalPriceOfBankTransferAsync(startOfMonth, today.AddDays(1));
            var totalPriceByCashThisMonth = await orderRepository.GetTotalPriceOfByCashAsync(startOfMonth, today.AddDays(1));

            // this year
            var numBankTransferThisYear = await orderRepository.GetNumberOfByBankTransferOrdersAsync(startOfYear, today.AddDays(1));
            var numCashThisYear = await orderRepository.GetNumberOfByCashOrdersAsync(startOfYear, today.AddDays(1));

            var totalPriceBankTransferThisYear = await orderRepository.GetTotalPriceOfBankTransferAsync(startOfYear, today.AddDays(1));
            var totalPriceByCashThisYear = await orderRepository.GetTotalPriceOfByCashAsync(startOfYear, today.AddDays(1));

            var result = new
            {
                ALL = new
                {
                    NumberOfBankTransferPayments = numBankTransferAll,
                    TotalPriceOfBankTransferPayments = totalPriceBankTransferAll,
                    NumberOfCashPayments = numCashAll,
                    TotalPriceOfCashPayments = totalPriceByCashAll,
                },
                Today = new
                {
                    NumberOfBankTransferPayments = numBankTransferToday,
                    TotalPriceOfBankTransferPayments = totalPriceBankTransferToday,
                    NumberOfCashPayments = numCashToday,
                    TotalPriceOfCashPayments = totalPriceByCashToday
                },
                ThisWeek = new
                {
                    NumberOfBankTransferPayments = numBankTransferThisWeek,
                    TotalPriceOfBankTransferPayments = totalPriceBankTransferThisWeek,
                    NumberOfCashPayments = numCashThisWeek,
                    TotalPriceOfCashPayments = totalPriceByCashThisWeek
                },
                ThisMonth = new
                {
                    NumberOfBankTransferPayments = numBankTransferThisMonth,
                    TotalPriceOfBankTransferPayments = totalPriceBankTransferThisMonth,
                    NumberOfCashPayments = numCashThisMonth,
                    TotalPriceOfCashPayments = totalPriceByCashThisMonth
                },
                ThisYear = new
                {
                    NumberOfBankTransferPayments = numBankTransferThisYear,
                    TotalPriceOfBankTransferPayments = totalPriceBankTransferThisYear,
                    NumberOfCashPayments = numCashThisYear,
                    TotalPriceOfCashPayments = totalPriceByCashThisYear
                }
            };

            return Ok(result);
        }


        /*        [HttpGet("GetTotalPriceOfTodayOrders")]
                public async Task<IActionResult> GetTotalPriceOfTodayOrders()
                {
                    var totalPrice = await orderRepository.GetTotalPriceOfTodayOrdersAsync();
                    return Ok(totalPrice);
                }

                [HttpGet("GetTotalPriceOfThisWeekOrders")]
                public async Task<IActionResult> GetTotalPriceOfThisWeekOrders()
                {
                    var totalPrice = await orderRepository.GetTotalPriceOfThisWeekOrdersAsync();
                    return Ok(totalPrice);
                }

                [HttpGet("GetTotalPriceOfThisMonthOrders")]
                public async Task<IActionResult> GetTotalPriceOfThisMonthOrders()
                {
                    var totalPrice = await orderRepository.GetTotalPriceOfThisMonthOrdersAsync();
                    return Ok(totalPrice);
                }*/

        [HttpGet("get-total-price-of-orders/{option}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetTotalPriceOfOrders(string option)
        {
            if (option == "today")
            {
                var totalPrice = await orderRepository.GetTotalPriceOfTodayOrdersAsync();
                return Ok(totalPrice);
            }
            else if (option == "this-week")
            {
                var totalPrice = await orderRepository.GetTotalPriceOfThisWeekOrdersAsync();
                return Ok(totalPrice);
            }
            else if (option == "this-month")
            {
                var totalPrice = await orderRepository.GetTotalPriceOfThisMonthOrdersAsync();
                return Ok(totalPrice);
            }
            else if (option == "this-year")
            {
                var totalPrice = await orderRepository.GetTotalPriceOfThisYearOrdersAsync();
                return Ok(totalPrice);
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }

        }

        [HttpGet("get-total-price-of-payment-type/{option}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetTotalPriceOfPaymentType(string option)
        {
            if (option == "bank-transfer")
            {
                var totalPriceBankTranfer = await orderRepository.GetTotalPriceOfBankTransferAsync();
                return Ok(totalPriceBankTranfer);
            }
            else if (option == "cash")
            {
                var totalPriceByCash = await orderRepository.GetTotalPriceOfByCashAsync();
                return Ok(totalPriceByCash);
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }

        }

        [HttpGet("get-number-of-status-order/{option}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetNumberOfStatusOrder(string option, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            if (option == "paid")
            {
                var count = await orderRepository.GetNumberPaidOrdersAsync(pageNumber, pageSize);
                return Ok(count);
            }
            else if (option == "done")
            {
                var count = await orderRepository.GetNumberDoneOrdersAsync(pageNumber, pageSize);
                return Ok(count);
            }
            else if (option == "waiting")
            {
                var count = await orderRepository.GetNumberWaitingOrdersAsync(pageNumber, pageSize);
                return Ok(count);
            }
            else if (option == "cancel")
            {
                var count = await orderRepository.GetNumberCancelOrdersAsync(pageNumber, pageSize);
                return Ok(count);
            }
            else if (option == "paying")
            {
                var count = await orderRepository.GetNumberPayingOrdersAsync(pageNumber, pageSize);
                return Ok(count);
            }
            else if (option == "expire")
            {
                var count = await orderRepository.GetNumberExpiredOrdersAsync(pageNumber, pageSize);
                return Ok(count);
            }
            else if (option == "wait")
            {
                var count = await orderRepository.GetNumberWaitOrdersAsync(pageNumber, pageSize);
                return Ok(count);
            }
            else if (option == "all")
            {
                var count = await orderRepository.GetNumberStatusOrdersAsync(pageNumber, pageSize);
                return Ok(count);
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }
        }



    }
}
