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
    public class ChequeController : ControllerBase
    {

        private readonly IChequeRepository _chequeRepository;

        public ChequeController(IChequeRepository chequeRepository)
        {
            _chequeRepository = chequeRepository;
        }

        // GET api/Cheque
        [HttpGet]
        public IActionResult Get()
        {
            var cheque = _chequeRepository.Query();

            return Ok(cheque);
        }

        // GET api/Cheque
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var cheque = await _chequeRepository.GetAsync(id);

            if (cheque != null)
            {
                return Ok(cheque);
            }
            else
                return BadRequest();
        }

        // POST api/Cheque
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cheque value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _chequeRepository.InsertAsync(value);

            return Created($"cheque/{value.ChequeId}", value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Cheque value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.ChequeId) return BadRequest();

            await _chequeRepository.UpdateAsync(value);

            return Ok(value);
        }


        // DELETE api/Cheque
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cheque = await _chequeRepository.DeleteAsync(id);

            return Ok(cheque);
        }
    }
}