using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoanApi.Models;
using LoanApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ICustomerRepository _customerRepository;
        private readonly ISequenceRepository _sequenceRepository;
        private readonly ILocationRepository _locationRepository;

        public CustomerController(ICustomerRepository customerRepository, ISequenceRepository sequenceRepository,
            ILocationRepository locationRepository)
        {
            _customerRepository = customerRepository;
            _sequenceRepository = sequenceRepository;
            _locationRepository = locationRepository;
        }

        // GET api/Customer
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var customer = _customerRepository.Query().Select(c=>new Customer {
                CustomerId = c.CustomerId, Code = c.Code, Date = c.Date,
                FullName = c.FullName, DateOfBirth = c.DateOfBirth, Gender = c.Gender,
                Mobile = c.Mobile, MaritalStatus = c.MaritalStatus, Type = c.Type,
                Location = _locationRepository.Query().FirstOrDefault(t => t.LocationId == c.LocationId)
            } );

            return Ok(customer);
        }

        // GET api/Customer
        [HttpGet("List")]
        public async Task<IActionResult> GetList()
        {
            var customer = _customerRepository.GetList();

            return Ok(customer);
        }

        // GET api/Customer
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var customer = await _customerRepository.GetAsync(id);

            if (customer != null)
            {
                return Ok(customer);
            }
            else
                return BadRequest();
        }

        // GET api/Customer
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode([FromRoute] string code)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var customer = _customerRepository.Query().FirstOrDefault(c=>c.Code == code);

            if (customer != null)
            {
                return Ok(customer);
            }
            else
                return BadRequest();
        }

        // POST api/Customer
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            value.Code = await _sequenceRepository.GetCode("Customer");

            await _customerRepository.InsertAsync(value);

            return Created($"customer/{value.CustomerId}", value);
        }

        // POST api/Customer/Batch
        [AllowAnonymous]
        [HttpPost("Batch")]
        public async Task<IActionResult> PostBatch([FromBody] Data value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //value.Code = await _sequenceRepository.GetCode("Customer");

            await _customerRepository.InsertRangeAsync(value.data);

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Customer has been added" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Customer value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.CustomerId) return BadRequest();

            await _customerRepository.UpdateAsync(value);

            return Ok(value);
        }
        
        // DELETE api/Customer
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var customer = await _customerRepository.DeleteAsync(id);
            if (customer != null)
            {
                return Ok(customer);
            }
            else
                return BadRequest();
        }
        
        public class Data
        {
            public List<Customer> data { get; set; }
        }
    }
}