using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Claims;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff,Manager")]
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[AllowAnonymous]
    public class CartController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, List<CartItem>> Carts = new();
        private readonly IMapper _mapper;
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IOrderRepository orderRepository;
        private readonly IWarrantyRepository warrantyRepository;

        public CartController(IMapper mapper, JewelrySalesSystemDbContext dbContext, IOrderRepository orderRepository, IWarrantyRepository warrantyRepository)
        {
            _mapper = mapper;
            this.dbContext = dbContext;
            this.orderRepository = orderRepository;
            this.warrantyRepository = warrantyRepository;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User not logged in");
            }

            var product = await dbContext.Products.FindAsync(cartItem.ProductId);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }
            else if (product.Status == "Dừng bán")
            {
                return BadRequest("Sản phầm dừng bán");
            }


            var userCart = Carts.GetOrAdd(userName, new List<CartItem>());

            var existingItem = userCart.FirstOrDefault(item => item.ProductId == cartItem.ProductId);
            int totalRequestedQuantity = existingItem != null ? existingItem.Quantity + cartItem.Quantity : cartItem.Quantity;

            if (totalRequestedQuantity > product.Quantity)
            {
                return BadRequest($"Không đủ hàng cho sản phẩm {product.ProductName}. Số lượng có sẵn: {product.Quantity}");
            }

            if (existingItem != null)
            {
                existingItem.Quantity += cartItem.Quantity;
            }
            else
            {
                cartItem.AddedDate = DateTime.Now;
                userCart.Add(cartItem);
            }

            var cartItemDTOs = userCart.Select(item =>
            {
                var productDTO = _mapper.Map<ProductDTO>(dbContext.Products.Find(item.ProductId));
                return new CartItemDTO
                {
                    ProductId = item.ProductId,
                    ProductName = productDTO.ProductName,
                    ProductCode = productDTO.ProductCode,
                    Image = productDTO.Image,
                    Quantity = item.Quantity,
                    SubTotal = item.Quantity * productDTO.Price,
                    AddedDateFormatted = item.AddedDate.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }).ToList();

            return Ok(cartItemDTOs);
        }


        [HttpGet]
        [Route("items")]
        public async Task<IActionResult> GetCartItems()
        {

            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User not logged in");
            }

            Carts.TryGetValue(userName, out var userCart);

            var cartItemDTOs = new List<CartItemDTO>();
            if (userCart != null)
            {
                foreach (var item in userCart)
                {
                    var product = await dbContext.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        var productDTO = _mapper.Map<ProductDTO>(product);
                        cartItemDTOs.Add(new CartItemDTO
                        {
                            ProductId = item.ProductId,
                            ProductName = productDTO.ProductName,
                            ProductCode = productDTO.ProductCode,
                            Image = productDTO.Image,
                            Quantity = item.Quantity,
                            SubTotal = item.Quantity * product.Price,
                            AddedDateFormatted = item.AddedDate.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                }
            }

            return Ok(cartItemDTOs);
        }
        [HttpDelete]
        [Route("clear")]
        public IActionResult ClearCart()
        {

            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User not logged in");
            }

            if (Carts.TryGetValue(userName, out var userCart))
            {
                userCart.Clear();
                return Ok("Cart cleared successfully");
            }

            return BadRequest("Cart is already empty");
        }

        [HttpDelete]
        [Route("remove/{productId}")]
        public IActionResult RemoveCartItem(int productId, int quantity)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User not logged in");
            }

            if (Carts.TryGetValue(userName, out var userCart))
            {
                var itemToRemove = userCart.FirstOrDefault(item => item.ProductId == productId);
                if (itemToRemove != null)
                {
                    if (quantity > 0 && quantity <= itemToRemove.Quantity)
                    {
                        if (quantity < itemToRemove.Quantity)
                        {
                            itemToRemove.Quantity -= quantity;
                        }
                        else
                        {
                            userCart.Remove(itemToRemove);
                        }
                        return Ok("Xóa sản phẩm thành công");
                    }
                    else
                    {
                        return BadRequest("Số lượng sản phẩm xóa không phù hợp");
                    }
                }
                else
                {
                    return BadRequest("Sản phẩm không tồn tại");
                }
            }

            return Ok();
        }


        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout([FromForm] CheckOutCustomerDTO customers)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not logged in");
            }

            string userName = userIdClaim.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Invalid user name");
            }

            var customer = await dbContext.Customers.FirstOrDefaultAsync(x =>
      (customers.PhoneNumberCustomer != null && x.PhoneNumber == customers.PhoneNumberCustomer) ||
      (customers.CustomerName != null && x.CustomerName == customers.CustomerName) ||
      (customers.Email != null && x.Email == customers.Email));

            var customerData = new
            {
                CustomerName = customer?.CustomerName ?? customers.CustomerName,
                PhoneNumber = customer?.PhoneNumber ?? customers.PhoneNumberCustomer,
                Email = customer?.Email ?? customers.Email,
                CustomerId = customer?.CustomerId
            };

            if (!Carts.TryGetValue(userName, out var userCart) || userCart.Count == 0)
            {
                return BadRequest("Giỏ hàng đang trống không thể thực hiện tạo đơn hàng");
            }

            double totalPrice = 0;
            foreach (var item in userCart)
            {
                //var product = await dbContext.Products.FindAsync(item.ProductId);
                //if (product != null)
                //{
                //    totalPrice += product.Price * item.Quantity;
                //    product.Quantity -= item.Quantity;
                //}
                var product = await dbContext.Products.FindAsync(item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    return BadRequest($"Không đủ hàng cho sản phẩm {product?.ProductName}. Số lượng có sẵn: {product?.Quantity}");
                }
                totalPrice += product.Price * item.Quantity;
                product.Quantity -= item.Quantity;
            }

            var order = new Orders
            {
                UserName = userName,
                CustomerName = customerData.CustomerName,
                PhoneNumber = customerData.PhoneNumber,
                Email = customerData.Email,
                Total = totalPrice,
                OrderDate = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time"),
                CustomerId = customerData.CustomerId,
                SaleById = userName,
                CashierId = null,
                ServicerId = null,
                Status = "Đợi thanh toán",
                OrderCode = await orderRepository.GenerateOrderCodeAsync()
            };


            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            var orderDetail = new OrderDetails();
            var checkoutItems = new List<CheckOutDTO>();
            var warranties = new List<WarrantyDTO>();
            double grandTotal = 0;
            foreach (var item in userCart)
            {
                var product = await dbContext.Products.FindAsync(item.ProductId);

                orderDetail = new OrderDetails
                {
                    ProductId = product.ProductId,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    OrderId = order.OrderId
                };
                dbContext.OrderDetails.Add(orderDetail);
                await dbContext.SaveChangesAsync();

                var subTotal = product.Price * item.Quantity;
                grandTotal += subTotal;
                var checkoutItem = new CheckOutDTO
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Quantity = item.Quantity,
                    OrderCode = order.OrderCode,
                    CustomerId = order.CustomerId,
                    PhoneNumber = customerData.PhoneNumber,
                    CustomerName = customerData.CustomerName,
                    Email = customerData.Email,
                    Price = product.Price,
                    SubTotal = subTotal,
                    OrderId = order.OrderId,
                    SaleById = userName,
                    //CashierId = userName,
                    //ServicerId = userName,
                };
                checkoutItems.Add(checkoutItem);
            }
            await dbContext.SaveChangesAsync();

            var checkoutResponse = new CheckOutResponseDTO
            {
                Bill = checkoutItems,
                Total = grandTotal,
                //Warranties = warranties
            };
            userCart.Clear();
            return Ok(checkoutResponse);
        }




        [HttpPost]
        [Route("addWarranty")]
        public async Task<IActionResult> AddWarranty([FromBody] AddWarrantyDTO addWarrantyDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User not logged in");
            }

            string userName = userIdClaim.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Invalid user name");
            }

            var orderCheck = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == addWarrantyDTO.OrderId);

            if (orderCheck == null)
            {
                return BadRequest("Order not found for this customer");
            }

            var orderDetails = await dbContext.OrderDetails
                                              .Include(od => od.Products)
                                              .Include(x => x.Products.Stones)
                                              .Include(x => x.Products.GoldTypes)
                                              .Include(x => x.Products.Categories)
                                              .Include(x => x.Orders.Customers)
                                              .Where(od => od.OrderId == addWarrantyDTO.OrderId)
                                              .ToListAsync();

            if (!orderDetails.Any())
            {
                return BadRequest("Order details not found");
            }

            // Get the highest existing warranty code
            var highestWarrantyCode = await dbContext.Warranties
                                                     .OrderByDescending(w => w.WarrantyCode)
                                                     .Select(w => w.WarrantyCode)
                                                     .FirstOrDefaultAsync();
            int highestWarrantyNumber = 0;

            if (highestWarrantyCode != null && highestWarrantyCode.StartsWith("PBH"))
            {
                int.TryParse(highestWarrantyCode.Substring(3), out highestWarrantyNumber);
            }

            var warranties = new List<Warranty>();
            int warrantyCounter = highestWarrantyNumber + 1;

            foreach (var orderDetail in orderDetails)
            {
                var totalPaid = await dbContext.Payments
                                               .Where(p => p.OrderId == addWarrantyDTO.OrderId)
                                               .SumAsync(p => (p.Cash ?? 0) + (p.BankTransfer ?? 0));
                if (totalPaid < orderDetail.Orders.Total && orderDetail.Orders.Status != "Đã thanh toán")
                {
                    return BadRequest("Order has not been fully paid for. Cannot add warranties.");
                }

                var existingWarrantiesCount = await dbContext.Warranties
                                                             .CountAsync(w => w.OrderDetailId == orderDetail.OrderDetailId);

                if (existingWarrantiesCount >= orderDetail.Quantity)
                {
                    return BadRequest($"Warranties already added for all quantities of product with OrderDetailId: {orderDetail.OrderDetailId}");
                }

                for (int i = existingWarrantiesCount; i < orderDetail.Quantity; i++)
                {
                    string warrantyCode = $"PBH{warrantyCounter:D4}";
                    warrantyCounter++;

                    var warranty = new Warranty
                    {
                        OrderDetailId = orderDetail.OrderDetailId,
                        CustomerId = orderDetail.Orders.CustomerId,
                        StartDate = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time"),
                        EndDate = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time").AddMonths(orderDetail.Products.WarrantyPeriod),
                        Status = addWarrantyDTO.Status,
                        WarrantyCode = warrantyCode
                    };

                    dbContext.Warranties.Add(warranty);
                    warranties.Add(warranty);
                }
            }

            await dbContext.SaveChangesAsync();

            var warrantyDTOs = warranties.Select(w => new WarrantyDTO
            {
                WarrantyCode = w.WarrantyCode,
                WarrantyId = w.WarrantyId,
                CustomerId = w.CustomerId,
                OrderDetailId = w.OrderDetailId,
                StartDate = w.StartDate,
                EndDate = w.EndDate,
                ServicerId = userName,
                Status = w.Status,
                OrderDetails = w.OrderDetails,
            }).ToList();

            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == addWarrantyDTO.OrderId);
            if (order != null)
            {
                order.ServicerId = userName;
                order.Status = "Đã hoàn thành";
                dbContext.Orders.Update(order);
                await dbContext.SaveChangesAsync();
            }

            return Ok(warrantyDTOs);
        }









        [HttpGet]
        [Route("get-warranty/{orderId}")]
        public async Task<IActionResult> GetWarranty(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User not logged in");
            }

            string userName = userIdClaim.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Invalid user name");
            }

            var warranties = await dbContext.Warranties
                .Include(w => w.OrderDetails)
                .ThenInclude(od => od.Products)
                .ThenInclude(p => p.Stones)
                .Include(w => w.OrderDetails)
                .ThenInclude(od => od.Products)
                .ThenInclude(p => p.GoldTypes)
                .Include(w => w.OrderDetails)
                .ThenInclude(od => od.Products)
                .ThenInclude(p => p.Categories)
                .Include(w => w.OrderDetails)
                .ThenInclude(od => od.Orders)
                .ThenInclude(o => o.Users)
                .ThenInclude(u => u.Roles)
                .Include(w => w.Customers)
                .Where(w => w.OrderDetails.OrderId == orderId)
                .ToListAsync();

            var warrantyDTOs = warranties.Select(w =>
            {
                var product = w.OrderDetails.Products;
                var endDate = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time").AddMonths(product.WarrantyPeriod);

                return new WarrantyDTO
                {
                    WarrantyCode = w.WarrantyCode,
                    WarrantyId = w.WarrantyId,
                    CustomerId = w.CustomerId,
                    OrderDetailId = w.OrderDetailId,
                    StartDate = TimeHelper.GetCurrentTimeInTimeZone("SE Asia Standard Time"),
                    EndDate = endDate,
                    Status = w.Status,
                    Customers = w.Customers,
                    OrderDetails = w.OrderDetails
                };
            }).ToList();

            return Ok(warrantyDTOs);
        }

    }
}
