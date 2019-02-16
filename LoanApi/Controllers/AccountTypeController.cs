using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoanApi.Models;
using LoanApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountTypeController : ControllerBase
    {

        private readonly IAccountTypeRepository _accounttypeRepository;
        private readonly ISequenceRepository _sequenceRepository;

        public AccountTypeController(IAccountTypeRepository accounttypeRepository, ISequenceRepository sequenceRepository)
        {
            _accounttypeRepository = accounttypeRepository;
            _sequenceRepository = sequenceRepository;
        }

        // GET api/AccountType
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var accounttype = _accounttypeRepository.Query();

            return Ok(accounttype);
        }

        // GET api/AccountType
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var accounttype = await _accounttypeRepository.GetAsync(id);

            if (accounttype != null)
            {
                return Ok(accounttype);
            }
            else
                return BadRequest();
        }

        // POST api/AccountType
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AccountType value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if(value.Code == null) value.Code = await _sequenceRepository.GetCode("AccountType");

            await _accounttypeRepository.InsertAsync(value);

            return Created($"accounttype/{value.AccountTypeId}", value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] AccountType value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.AccountTypeId) return BadRequest();
            if (value.Code == null) value.Code = await _sequenceRepository.GetCode("AccountType");

            await _accounttypeRepository.UpdateAsync(value);

            return Ok(value);
        }


        // DELETE api/AccountType
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var accounttype = await _accounttypeRepository.DeleteAsync(id);

            return Ok(accounttype);
        }
    }
}