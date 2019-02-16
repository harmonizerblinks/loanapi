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
    public class SessionController : ControllerBase
    {

        private readonly ISessionRepository _sessionRepository;
        private readonly ICompanyRepository _companyRepository;

        public SessionController(ISessionRepository sessionRepository, ICompanyRepository companyRepository)
        {
            _sessionRepository = sessionRepository;
            _companyRepository = companyRepository;
        }

        // GET api/Session
        [HttpGet]
        public IActionResult Get()
        {
            var session = _sessionRepository.Query().OrderByDescending(d=>d.Date);

            return Ok(session);
        }

        // GET api/Session
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var session = await _sessionRepository.GetAsync(id);

            if (session != null)
            {
                return Ok(session);
            }
            else
                return BadRequest();
        }

        // GET api/Session
        [HttpGet("Close/{id}/{userid}")]
        public async Task<IActionResult> GetById([FromRoute] int id, string userid)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var session = await _sessionRepository.GetAsync(id);

            if (session != null)
            {
                return Ok(session);
            }
            else
                return BadRequest();
        }

        // POST api/Session
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Session value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ses = _sessionRepository.Query().Where(s => s.Status == "Active").LastOrDefault();
            if(ses != null)
            {
                ses.Status = "Closed"; ses.MDate = DateTime.UtcNow; ses.MUserId = value.UserId;
                await _sessionRepository.UpdateAsync(ses);
            }
            var company = _companyRepository.Query().LastOrDefault();
            company.SessionDate = value.SessionDate;

            await _sessionRepository.InsertAsync(value);
            await _companyRepository.UpdateAsync(company);

            return Created($"session/{value.SessionId}", value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Session value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.SessionId) return BadRequest();

            await _sessionRepository.UpdateAsync(value);

            return Ok(value);
        }
        
        // DELETE api/Session
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var session = await _sessionRepository.DeleteAsync(id);

            return Ok(session);
        }
    }
}