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
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISequenceRepository _sequenceRepository;

        public EmployeeController(IEmployeeRepository employeeRepository, ISequenceRepository sequenceRepository)
        {
            _employeeRepository = employeeRepository;
            _sequenceRepository = sequenceRepository;
        }

        // GET api/Employee
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var employee = _employeeRepository.Query();

            return Ok(employee);
        }

        // GET api/Employee
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var employee = await _employeeRepository.GetAsync(id);

            if (employee != null)
            {
                return Ok(employee);
            }
            else
                return BadRequest();
        }

        // POST api/Employee
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            value.Code = await _sequenceRepository.GetCode("Employee");

            await _employeeRepository.InsertAsync(value);

            return Created($"employee/{value.EmployeeId}", value);
        }

        // PUT api/Employee
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Employee value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.EmployeeId) return BadRequest();

            await _employeeRepository.UpdateAsync(value);

            return Ok(value);
        }

        // DELETE api/Employee
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var employee = await _employeeRepository.DeleteAsync(id);

            return Ok(employee);
        }

    }
}