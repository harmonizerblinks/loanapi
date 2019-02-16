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
    public class AlertController : ControllerBase
    {
        private readonly IAlertRepository _alertRepository;

        public AlertController(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
        }

        // GET api/Alert
        [HttpGet]
        public IActionResult Get()
        {
            var Alert = _alertRepository.Query();

            return Ok(Alert);
        }

        // GET api/Alert
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var Alert = await _alertRepository.GetAsync(id);

            if (Alert != null)
            {
                return Ok(Alert);
            }
            else
                return BadRequest();
        }

        // GET api/Alert
        [HttpGet("Type/{type}")]
        public async Task<IActionResult> GetByType([FromRoute] string type)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var alert = _alertRepository.Query().Where(t=>t.Type == type).FirstOrDefault();

            if (alert != null)
            {
                return Ok(alert);
            }
            else
                return BadRequest();
        }

        // POST api/Alert
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Alert value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var alert = _alertRepository.Query().FirstOrDefault(t => t.Type == value.Type);

            if (alert != null)
            {
                alert.Message = value.Message; alert.Name = value.Name;
                alert.Auto = value.Auto; value.AlertId = alert.AlertId;
                await _alertRepository.UpdateAsync(alert);
            } else {
                await _alertRepository.InsertAsync(value);
            }

            return Created($"Alert/{value.AlertId}", value);
        }

        // PUT api/Alert
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Alert value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.AlertId) return BadRequest();

            await _alertRepository.UpdateAsync(value);

            return Ok(value);
        }

        // DELETE api/Alert
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var Alert = await _alertRepository.DeleteAsync(id);

            return Ok(Alert);
        }

    }
}