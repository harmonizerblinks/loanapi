using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LoanApi.Models;
using LoanApi.Repository;

namespace LoanApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class TransitController : ControllerBase
    {
        private readonly ITransitRepository _transitRepository;
        private readonly ISequenceRepository _sequenceRepository;

        public TransitController(ITransitRepository transitRepository, ISequenceRepository sequenceRepository)
        {
            _transitRepository = transitRepository;
            _sequenceRepository = sequenceRepository;
        }

        // GET api/Transit
        [HttpGet]
        public IActionResult Get()
        {
            var transit = _transitRepository.Query();

            return Ok(transit);
        }

        // GET api/Transit
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var transit = await _transitRepository.GetAsync(id);

            if (transit != null)
            {
                return Ok(transit);
            }
            else
                return BadRequest();
        }

        // POST api/Transit
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Transit value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            value.Code = await _sequenceRepository.GetCode("Transit");

            await _transitRepository.InsertAsync(value);

            return Created($"transit/{value.TransitId}", value);
        }

        // PUT api/Transit
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Transit value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.TransitId) return BadRequest();

            await _transitRepository.UpdateAsync(value);

            return Ok(value);
        }

        // DELETE api/Transit
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var transit = await _transitRepository.DeleteAsync(id);

            return Ok(transit);
        }
    }
}