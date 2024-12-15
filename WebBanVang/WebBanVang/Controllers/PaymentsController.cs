using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
    public class PaymentsController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IPaymentRepository paymentRepository;
        private readonly IMapper mapper;
        private readonly BlobServiceClient blobServiceClient;

        public PaymentsController(JewelrySalesSystemDbContext dbContext,
            IPaymentRepository paymentRepository, IMapper mapper, BlobServiceClient blobServiceClient)
        {
            this.dbContext = dbContext;
            this.paymentRepository = paymentRepository;
            this.mapper = mapper;
            this.blobServiceClient = blobServiceClient;
        }

        // GET: api/Payments
        [HttpGet]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetAllPayment()

        {
            var paymentDomain = await paymentRepository.GetAllPayment();
            return Ok(mapper.Map<List<PaymentDTO>>(paymentDomain));
        }
        [HttpGet("GetPaymentByOption/option")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetPaymentsByOption([FromQuery] string option)
        {
            List<Payment> payments;

            switch (option.ToLower())
            {
                case "today":
                    payments = await paymentRepository.GetPaymentsToday();
                    break;
                case "this-week":
                    payments = await paymentRepository.GetPaymentsThisWeek();
                    break;
                case "this-month":
                    payments = await paymentRepository.GetPaymentsThisMonth();
                    break;
                case "this-year":
                    payments = await paymentRepository.GetPaymentsThisYear();
                    break;
                default:
                    return BadRequest("Invalid option. Valid options are 'today', 'this-week', 'this-month','this-year'.");
            }

            return Ok(mapper.Map<List<PaymentDTO>>(payments));
        }
        // GET: api/Payments/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var paymentDomain = await paymentRepository.GetPaymentById(id);
            if (paymentDomain == null) return NotFound();
            return Ok(mapper.Map<PaymentDTO>(paymentDomain));
        }


        [HttpGet("GetPaymentByOrderOrderCode")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetPaymentByOrderId(string orderOrder)
        {
            var paymentDomain = await paymentRepository.GetPaymentByOrderCode(orderOrder);
            if (paymentDomain == null) return NotFound();
            return Ok(mapper.Map<List<PaymentDTO>>(paymentDomain));
        }



        [HttpGet("SearchPaymentByPaymentCodeOrOrderCode")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> SearchPaymentByPaymentCodeOrOrderCode(string searchText)
        {
            var paymentDomain = await paymentRepository.SearchPaymentByPaymentCodeOrOrderCode(searchText);
            if (paymentDomain == null) return NotFound();
            return Ok(mapper.Map<List<PaymentDTO>>(paymentDomain));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdatePayment(int id, [FromForm] UpdatePaymentDTO updatePaymentDTO)
        {


            var existingPayment = await paymentRepository.GetPaymentById(id);
            if (existingPayment == null)
            {
                return NotFound();
            }

            // Map the updated fields from the DTO to the domain model
            existingPayment.PaymentCode = updatePaymentDTO.PaymentCode;
            existingPayment.PaymentType = updatePaymentDTO.PaymentType;
            existingPayment.Cash = updatePaymentDTO.Cash;
            existingPayment.BankTransfer = updatePaymentDTO.BankTransfer;
            existingPayment.TransactionId = updatePaymentDTO.TransactionId;
            existingPayment.PaymentTime = updatePaymentDTO.PaymentTime;
            existingPayment.Status = updatePaymentDTO.Status;
            if (updatePaymentDTO.Image == null)
            {
                existingPayment.Image = null;
            }
            // Handle image upload if a new image is provided
            if (updatePaymentDTO.Image != null)
            {
                var containerClient = blobServiceClient.GetBlobContainerClient("paymentcontainer");

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(existingPayment.Image))
                {
                    var oldBlobName = Path.GetFileName(new Uri(existingPayment.Image).AbsolutePath);
                    var oldBlobClient = containerClient.GetBlobClient(oldBlobName);
                    await oldBlobClient.DeleteIfExistsAsync();
                }

                // Upload the new image
                var newBlobClient = containerClient.GetBlobClient(updatePaymentDTO.Image.FileName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = updatePaymentDTO.Image.ContentType
                };

                using (var stream = updatePaymentDTO.Image.OpenReadStream())
                {
                    await newBlobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
                }

                // Update the Image URL in the domain model
                existingPayment.Image = newBlobClient.Uri.ToString();
            }

            // Update the payment in the repository
            await paymentRepository.UpdatePayment(id, existingPayment);

            return Ok(mapper.Map<PaymentDTO>(existingPayment));
        }

        // POST: api/Payments
        [HttpPost]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<ActionResult<Payment>> CreatePayment([FromForm] AddPaymentDTO addPaymentDTO)
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

            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == addPaymentDTO.OrderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            //var totalPaid = await dbContext.Payments.Where(p => p.OrderId == addPaymentDTO.OrderId).SumAsync(p => (p.Cash ?? 0) + (p.BankTransfer ?? 0));
            //if (totalPaid >= order.Total)
            //{
            //    return BadRequest("Order is already fully paid. No further payments are allowed.");
            //}

            double totalpayment = (addPaymentDTO.Cash ?? 0) + (addPaymentDTO.BankTransfer ?? 0);
            if (totalpayment + await dbContext.Payments.Where(p => p.OrderId == addPaymentDTO.OrderId).SumAsync(p => (p.Cash ?? 0) + (p.BankTransfer ?? 0)) == order.Total)
            {
                addPaymentDTO.Status = "Đã thanh toán";
            }
            else
            {
                addPaymentDTO.Status = "Đang thanh toán";
            }

            order.CashierId = userName;
            dbContext.Orders.Update(order);
            await dbContext.SaveChangesAsync();

            var paymentDomainModel = mapper.Map<Payment>(addPaymentDTO);
            if (paymentDomainModel == null)
            {
                return BadRequest("Payment mapping failed");
            }

            if (addPaymentDTO.Image != null)
            {
                var containerClient = blobServiceClient.GetBlobContainerClient("paymentcontainer");
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(addPaymentDTO.Image.FileName);
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = addPaymentDTO.Image.ContentType
                };
                using (var stream = addPaymentDTO.Image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
                }
                paymentDomainModel.Image = blobClient.Uri.ToString();
            }

            paymentDomainModel = await paymentRepository.AddPayment(paymentDomainModel);
            await paymentRepository.UpdateStatusOrder(addPaymentDTO.OrderId);

            return Ok(mapper.Map<PaymentDTO>(paymentDomainModel));
        }


        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            // Fetch the payment details
            var paymentDomain = await paymentRepository.GetPaymentById(id);
            if (paymentDomain == null)
            {
                return NotFound();
            }

            // Delete the image from Azure Blob Storage if it exists
            if (!string.IsNullOrEmpty(paymentDomain.Image))
            {
                var containerClient = blobServiceClient.GetBlobContainerClient("paymentcontainer");
                var blobName = Path.GetFileName(new Uri(paymentDomain.Image).AbsolutePath);
                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();
            }

            // Delete the payment from the database
            var checkExist = await paymentRepository.DeletePayment(id);
            if (checkExist == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<PaymentDTO>(checkExist));
        }



        [HttpGet("get-payments-by-payment-type/{option}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetPaymentsByPaymentType(string option)
        {
            if (option == "bank-transfer")
            {
                var paymentDomain = await paymentRepository.GetBankTransferPayment();
                return Ok(mapper.Map<List<PaymentDTO>>(paymentDomain));
            }
            else if (option == "cash")
            {
                var paymentDomain = await paymentRepository.GetByCashPayment();
                return Ok(mapper.Map<List<PaymentDTO>>(paymentDomain));
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }
        }

    }
}
