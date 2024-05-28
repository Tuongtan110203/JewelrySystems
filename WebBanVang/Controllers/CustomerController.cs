using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000")]
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
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await customerRepository.GetAllCustomersAsync();
            return Ok(mapper.Map<List<CustomerDTO>>(customers));
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomersById(int id)
        {
            var customer = await customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CustomerDTO>(customer));
        }
        [HttpGet("GetCustomerByPhone")]
        public async Task<IActionResult> GetCustomerByNumberPhone(string phone)
        {
            var customer = await customerRepository.GetCustomerByNumberPhone(phone);
            if(customer == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CustomerDTO>(customer));
        }
        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomers(int id, UpdateCustomerDTO updateCustomerDTO)
        {

            var existing = await customerRepository.GetCustomerByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }
          
            var customerDomainModel = mapper.Map<Customers>(updateCustomerDTO);
            await customerRepository.UpdateCustomersAsync(id, customerDomainModel);

            return Ok(mapper.Map<CustomerDTO>(customerDomainModel));
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customers>> PostCustomers(AddCustomerDTO addCustomerDTO)
        {
            var customerModel = mapper.Map<Customers>(addCustomerDTO);

            if (customerModel == null)
            {
                return BadRequest("Invalid customer data.");
            }
            if (await customerRepository.IsPhoneNumberDuplicateAsync(customerModel.PhoneNumber))
            {
                return BadRequest("Phone number is already in use by another customer.");
            }
            customerModel = await customerRepository.CreateAsync(customerModel);
            return Ok(mapper.Map<CustomerDTO>(customerModel));
        }


        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomers(int id)
        {
            var checkExist = await customerRepository.DeleteCustomersAsync(id);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CustomerDTO>(checkExist));
        }
        
      
    }
}
