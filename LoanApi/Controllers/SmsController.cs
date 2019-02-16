using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LoanApi.Models;
using LoanApi.Repository;
using Hangfire;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace LoanApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly ISmsRepository _smsRepository;
        private readonly ISmsApiRepository _smsapiRepository;

        public SmsController(ISmsRepository smsRepository, ISmsApiRepository smsapiRepository)
        {
            _smsRepository = smsRepository;
            _smsapiRepository = smsapiRepository;
        }

        // GET api/Sms
        [HttpGet]
        public IActionResult Get()
        {
            var sms = _smsRepository.Query();

            return Ok(sms);
        }

        // GET api/Sms
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var sms = await _smsRepository.GetAsync(id);

            if (sms != null)
            {
                return Ok(sms);
            }
            else
                return BadRequest();
        }

        // GET Sms
        [HttpGet("Retry/{id}")]
        public async Task<IActionResult> RetrybyId([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var config = _smsapiRepository.Query().LastOrDefault(a => a.Status.ToLower().Contains("active") && a.Default == true);
            var sms = await _smsRepository.GetAsync(id);
            var httpClient = new HttpClient();
            StringBuilder sb = new StringBuilder();
            sb.Append(config.Url).Append("?cmd=sendquickmsg&owneremail=Harmonizerblinks@gmail.com&subacct=")
                .Append(config.Username).Append("&subacctpwd=").Append(config.Password)
                .Append("&message=").Append(sms.Message).Append("&sender=").Append(config.SenderId)
                .Append("&sendto=").Append(sms.Mobile).Append("&msgtype=0");
            var json = await httpClient.GetStringAsync(sb.ToString());
            // var smsresponse = JsonConvert.DeserializeObject<SmsResponse>(json);
            sms.Code = 0; sms.Response = json;

            await _smsRepository.UpdateAsync(sms);

            return Ok(sms);
        }

        // POST Sms
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Sms sms)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var config = _smsapiRepository.Query().LastOrDefault(a => a.Status.ToLower().Contains("active") && a.Default == true);
            try
            {
                var httpClient = new HttpClient();
                StringBuilder sb = new StringBuilder();
                sb.Append(config.Url).Append("?cmd=sendquickmsg&owneremail=Harmonizerblinks@gmail.com&subacct=")
                    .Append(config.Username).Append("&subacctpwd=").Append(config.Password)
                    .Append("&message=").Append(sms.Message).Append("&sender=").Append(config.SenderId)
                    .Append("&sendto=").Append(sms.Mobile).Append("&msgtype=0");
                var json = await httpClient.GetStringAsync(sb.ToString());
                // var smsresponse = JsonConvert.DeserializeObject<SmsResponse>(json);
                sms.Code = 0; sms.Response = json;
                sms.MDate = DateTime.UtcNow;

                await _smsRepository.InsertAsync(sms);
            }
            catch (Exception e)
            {
                sms.Code = 400; sms.Response = "Unable to send sms";
                await _smsRepository.InsertAsync(sms);
                BackgroundJob.Schedule(() => RetrybyId(sms.SmsId), TimeSpan.FromMinutes(3));

                return null;
            }
            return Created($"sms/{sms.SmsId}", sms);
        }

        // PUT api/Sms
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Sms value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.SmsId) return BadRequest();

            await _smsRepository.UpdateAsync(value);

            return Ok(value);
        }

        // DELETE api/Sms
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var sms = await _smsRepository.DeleteAsync(id);

            return Ok(sms);
        }
    }
}