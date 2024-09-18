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
    public class CustomerController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly ICustomerRepository customerRepository;
        private readonly IMapper mapper;

        public CustomerController(JewelrySalesSystemDbContext dbContext, ICustomerRepository customerRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.customerRepository = customerRepository;
            this.mapper = mapper;
        }

        // GET: api/Customers
        [HttpGet]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await customerRepository.GetAllCustomersAsync();
            return Ok(mapper.Map<List<CustomerDTO>>(customers));
        }
        [HttpGet("SearchCustomerByNameOrPhone")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> SearchCustomerByNameOrPhone([FromQuery] string searchText)
        {
            var customers = await customerRepository.SearchCustomersByPhoneOrFullname(searchText);
            if (customers == null || customers.Count == 0)
            {
                return NotFound();
            }
            return Ok(mapper.Map<List<CustomerDTO>>(customers));
        }
        // GET: api/Customers/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetCustomersById(int id)
        {
            var customer = await customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CustomerDTO>(customer));
        }

        [HttpGet("get-customers-by-phone-number")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetCustomersByNumberPhone(string phone)
        {
            var customer = await customerRepository.GetCustomersByNumberPhone(phone);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<List<CustomerDTO>>(customer));
        }
        [HttpGet("get-customer-by-phone-number")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetCustomerByNumberPhone(string phone)
        {
            var customer = await customerRepository.GetCustomerByNumberPhone(phone);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CustomerDTO>(customer));
        }
        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> PutCustomers(int id, UpdateCustomerDTO updateCustomerDTO)
        {
            var existingCustomer = await customerRepository.GetCustomerByIdAsync(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            if (await IsDuplicatePhoneNumber(updateCustomerDTO.PhoneNumber, id))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Số điện thoại đã tồn tại");
            }

            if (await IsDuplicateEmail(updateCustomerDTO.Email, id))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Email đã tồn tại");
            }

            var customerDomainModel = mapper.Map<Customers>(updateCustomerDTO);
            await customerRepository.UpdateCustomersAsync(id, customerDomainModel);

            return Ok(mapper.Map<CustomerDTO>(customerDomainModel));
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Manager,Staff")]
        public async Task<ActionResult<Customers>> PostCustomers(AddCustomerDTO addCustomerDTO)
        {
            if (await IsDuplicatePhoneNumber(addCustomerDTO.PhoneNumber))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Số điện thoại đã tồn tại");
            }
            if (await IsDuplicateEmail(addCustomerDTO.Email))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Email đã tồn tại");
            }
            var customerModel = mapper.Map<Customers>(addCustomerDTO);
            if (customerModel == null) { return NotFound(); }
            customerModel = await customerRepository.CreateAsync(customerModel);
            return Ok(mapper.Map<CustomerDTO>(customerModel));
        }


        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> DeleteCustomers(int id)
        {
            var checkExist = await customerRepository.DeleteCustomersAsync(id);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CustomerDTO>(checkExist));
        }

        private async Task<bool> IsDuplicatePhoneNumber(string phoneNumber, int? id = null)
        {
            return await customerRepository.IsPhoneNumberDuplicateAsync(phoneNumber, id);
        }

        private async Task<bool> IsDuplicateEmail(string email, int? id = null)
        {
            return await customerRepository.IsEmailDuplicateAsync(email, id);
        }


    }
}
