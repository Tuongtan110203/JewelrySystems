using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "staff")]
    [EnableCors("AllowLocalhost3000")]
    public class CartController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, List<CartItem>> Carts = new();
        private readonly IMapper _mapper;
        private readonly JewelrySalesSystemDbContext dbContext;

        public CartController(IMapper mapper, JewelrySalesSystemDbContext dbContext)
        {
            _mapper = mapper;
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem cartItem)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User not logged in");
            }

            var product = await dbContext.Products.FindAsync(cartItem.ProductId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var userCart = Carts.GetOrAdd(userName, new List<CartItem>());

            var existingItem = userCart.FirstOrDefault(item => item.ProductId == cartItem.ProductId);
            int totalRequestedQuantity = existingItem != null ? existingItem.Quantity + cartItem.Quantity : cartItem.Quantity;

            if (totalRequestedQuantity > product.Quantity)
            {
                return BadRequest($"Not enough stock for product {product.ProductName}. Available quantity: {product.Quantity}");
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
                        return Ok("Item removed successfully");
                    }
                    else
                    {
                        return BadRequest("Invalid quantity to remove");
                    }
                }
                else
                {
                    return BadRequest("Product not exist");
                }
            }

            return Ok();
        }


        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(CheckOutCustomerDTO customers)
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

            var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == customers.PhoneNumberCustomer);
            if (customer == null)
            {
                return BadRequest("Customer not found");
            }

            if (!Carts.TryGetValue(userName, out var userCart) || userCart.Count == 0)
            {
                return BadRequest("Cart is empty");
            }

            double totalPrice = 0;
            foreach (var item in userCart)
            {
                var product = await dbContext.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    totalPrice += product.Price * item.Quantity;
                    product.Quantity -= item.Quantity;
                }
            }
            var order = new Orders
            {
                UserName = userName,
                Total = totalPrice,
                OrderDate = DateTime.Now,
                CustomerId = customer.CustomerId,
                SaleById = userName,
                CashierId = userName,
                ServicerId = userName,
            };
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            var orderDetail = new OrderDetails();
            //foreach (var item in userCart)
            //{
            //    var product = await dbContext.Products.FindAsync(item.ProductId);
            //    orderDetail = new OrderDetails
            //    {
            //        ProductId = product.ProductId,
            //        Price = product.Price,
            //        Quantiry = item.Quantity,
            //        OrderId = order.OrderId
            //    };
            //    dbContext.OrderDetails.Add(orderDetail);
            //}
            //  await dbContext.SaveChangesAsync();
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
                    PhoneNumber = customer.PhoneNumber,
                    CustomerName = customer.CustomerName,
                    Price = product.Price,
                    SubTotal = subTotal,
                    OrderId = order.OrderId,
                    SaleById = userName,
                    CashierId = userName,
                    ServicerId = userName,
                };
                checkoutItems.Add(checkoutItem);


                //var warranty = new Warranty
                //{
                //    CustomerId = customer.CustomerId,
                //    OrderDetailId = orderDetail.OrderDetailId,
                //    StartDate = DateTime.Now,
                //    EndDate = DateTime.Now.AddMonths(product.WarrantyPeriod),
                //    Status = ""
                //};
                //dbContext.Warranties.Add(warranty);
                //   warranties.Add(_mapper.Map<WarrantyDTO>(warranty));

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

            var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == addWarrantyDTO.CustomerPhone);
            if (customer == null)
            {
                return BadRequest("Customer not found");
            }
            var orderCheck = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == addWarrantyDTO.OrderId && o.CustomerId == customer.CustomerId);
            if (orderCheck == null)
            {
                return BadRequest("Order not found for this customer");
            }
            var orderDetails = await dbContext.OrderDetails
                                               .Include(od => od.Products)
                                               .Where(od => od.OrderId == addWarrantyDTO.OrderId)
                                               .ToListAsync();
            if (!orderDetails.Any())
            {
                return BadRequest("Order details not found");
            }

            var warranties = new List<Warranty>();

            foreach (var orderDetail in orderDetails)
            {
                var warranty = new Warranty
                {
                    CustomerId = customer.CustomerId,
                    OrderDetailId = orderDetail.OrderDetailId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(orderDetail.Products.WarrantyPeriod),
                    Status = addWarrantyDTO.Status
                };

                dbContext.Warranties.Add(warranty);
                warranties.Add(warranty);
            }

            await dbContext.SaveChangesAsync();

            var warrantyDTOs = warranties.Select(w => new WarrantyDTO
            {
                WarrantyId = w.WarrantyId,
                CustomerId = w.CustomerId,
                OrderDetailId = w.OrderDetailId,
                StartDate = w.StartDate,
                EndDate = w.EndDate,
                Status = w.Status
            }).ToList();
            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == addWarrantyDTO.OrderId);
            if (order != null)
            {
                order.Status = "Đã hoàn thành";
                dbContext.Orders.Update(order);
                await dbContext.SaveChangesAsync();
            }

            return Ok(warrantyDTOs);
        }





        [HttpPost]
        [Route("Getwarranty")]
        public async Task<IActionResult> GetWarranty(PrintWarrantyDTO warranty)
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
                .Where(w => w.OrderDetails.Orders.UserName == userName && w.OrderDetails.OrderId == warranty.OrderId)
                .ToListAsync();

            var warrantyDTOs = warranties.Select(w =>
            {
                var product = w.OrderDetails.Products;
                var endDate = DateTime.Now.AddMonths(product.WarrantyPeriod);

                return new WarrantyDTO
                {
                    WarrantyId = w.WarrantyId,
                    CustomerId = w.CustomerId,
                    OrderDetailId = w.OrderDetailId,
                    StartDate = DateTime.Now,
                    EndDate = endDate,
                    Status = w.Status
                };
            }).ToList();

            return Ok(warrantyDTOs);
        }


    }
}
