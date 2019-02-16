using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LoanApi.Repository;
using LoanApi.Models;

namespace LoanApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ISequenceRepository _sequenceRepository;

        public LocationController(ILocationRepository locationRepository, ISequenceRepository sequenceRepository)
        {
            _locationRepository = locationRepository;
            _sequenceRepository = sequenceRepository;
        }

        // GET api/Location
        [HttpGet]
        public IActionResult Get()
        {
            var location = _locationRepository.GetAll();

            return Ok(location);
        }

        // GET api/Location
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var location = await _locationRepository.GetAsync(id);

            if (location != null)
            {
                return Ok(location);
            }
            else
                return BadRequest();
        }

        // POST api/Location
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Location value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            value.Code = await _sequenceRepository.GetCode("Location");

            await _locationRepository.InsertAsync(value);

            return Created($"location/{value.LocationId}", value);
        }

        // PUT api/Location
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Location value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.LocationId) return BadRequest();

            await _locationRepository.UpdateAsync(value);

            return Ok(value);
        }

        // DELETE api/Location
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var location = await _locationRepository.DeleteAsync(id);

            return Ok(location);
        }
    }
}