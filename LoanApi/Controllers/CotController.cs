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
    public class CotController : ControllerBase
    {

        private readonly ICotRepository _cotRepository;

        public CotController(ICotRepository cotRepository)
        {
            _cotRepository = cotRepository;
        }

        // GET api/Cot
        [HttpGet]
        public IActionResult Get()
        {
            var cot = _cotRepository.GetAll();

            return Ok(cot);
        }

        // GET api/Cot
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var cot = await _cotRepository.GetAsync(id);

            if (cot != null)
            {
                return Ok(cot);
            }
            else
                return BadRequest();
        }

        // POST api/Cot
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cot value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _cotRepository.InsertAsync(value);

            return Created($"cot/{value.CotId}", value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Cot value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.CotId) return BadRequest();

            await _cotRepository.UpdateAsync(value);

            return Ok(value);
        }


        // DELETE api/Cot
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cot = await _cotRepository.DeleteAsync(id);

            return Ok(cot);
        }
    }
}