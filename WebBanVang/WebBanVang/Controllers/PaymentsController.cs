using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;


namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000")]
    public class PaymentsController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IPaymentRepository paymentRepository;
        private readonly IMapper mapper;

        public PaymentsController(JewelrySalesSystemDbContext dbContext,IPaymentRepository paymentRepository,IMapper mapper)
        {
            dbContext = dbContext;
            this.paymentRepository = paymentRepository;
            this.mapper = mapper;
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<IActionResult> GetAllPayment()
        {
            var paymentDomain = await paymentRepository.GetAllPayment();
            return Ok(mapper.Map<List<PaymentDTO>>(paymentDomain));
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var paymentDomain = await paymentRepository.GetPaymentById(id);
            if (paymentDomain == null) NotFound();
            return Ok(mapper.Map<PaymentDTO>(paymentDomain));
        }

        // PUT: api/Payments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, UpdatePaymentDTO updatePaymentDTO)
        {
            var existingPayment = await paymentRepository.GetPaymentById(id);
            if (existingPayment == null)
            {
                return NotFound();
            }

            var paymentDomainModel = mapper.Map<Payment>(updatePaymentDTO);
            await paymentRepository.UpdatePayment(id, paymentDomainModel);

            return Ok(mapper.Map<PaymentDTO>(paymentDomainModel));
        }

        // POST: api/Payments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/Payments
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment([FromBody] AddPaymentDTO addPaymentDTO)
        {
            var paymentDomainModel = mapper.Map<Payment>(addPaymentDTO);
            if (paymentDomainModel == null) { return NotFound(); }
           
            paymentDomainModel = await paymentRepository.AddPayment(paymentDomainModel);
        //    await paymentRepository.UpdateStatusWarranty(addPaymentDTO.OrderId);
            await paymentRepository.UpdateStatusOrder(addPaymentDTO.OrderId);

            return Ok(mapper.Map<PaymentDTO>(paymentDomainModel));
        }



        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var checkExist = await paymentRepository.DeletePayment(id);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<PaymentDTO>(checkExist));
        }

       
    }
}
