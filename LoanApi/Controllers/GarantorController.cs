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
    public class GarantorController : ControllerBase
    {
        private readonly IGarantorRepository _garantorRepository;

        public GarantorController(IGarantorRepository garantorRepository)
        {
            _garantorRepository = garantorRepository;
        }

        // GET api/Garantor
        [HttpGet]
        public IActionResult Get()
        {
            var garantor = _garantorRepository.Query();

            return Ok(garantor);
        }

        // GET api/Garantor
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var garantor = await _garantorRepository.GetAsync(id);

            if (garantor != null)
            {
                return Ok(garantor);
            }
            else
                return BadRequest();
        }

        // POST api/Garantor
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Garantor value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _garantorRepository.InsertAsync(value);

            return Created($"garantor/{value.GarantorId}", value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Garantor value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.GarantorId) return BadRequest();

            await _garantorRepository.UpdateAsync(value);

            return Ok(value);
        }


        // DELETE api/Garantor
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var garantor = await _garantorRepository.DeleteAsync(id);
            if (garantor != null)
            {
                return Ok(garantor);
            }
            else
                return BadRequest();
        }
    }
}