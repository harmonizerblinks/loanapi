using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LoanApi.Models;
using LoanApi.Repository;
using System;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace LoanApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class TellerController : ControllerBase
    {
        private readonly ITellerRepository _tellerRepository;
        private readonly INominalRepository _nominalRepository;
        private readonly ISequenceRepository _sequenceRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TellerController(ITellerRepository tellerRepository, ISequenceRepository sequenceRepository,
            INominalRepository nominalRepository, ITransactionRepository transactionRepository)
        {
            _tellerRepository = tellerRepository;
            _nominalRepository = nominalRepository;
            _sequenceRepository = sequenceRepository;
            _transactionRepository = transactionRepository;
        }

        // GET api/Teller
        [HttpGet]
        public IActionResult Get()
        {
            var teller = _tellerRepository.GetAll().Select(t => new {
                t.TellerId,
                t.Nominal.Code,
                t.Id,
                Name = t.Nominal.Description,
                User = t.AppUser.UserName,
                NoOfTrans = t.Transactions.Where(c => c.UserId == t.Id).Count()
            }).ToList();

            return Ok(teller);
        }

        // GET api/Teller
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var teller = await _tellerRepository.GetAsync(id);

            if (teller != null)
            {
                return Ok(teller);
            }
            else
                return BadRequest();
        }

        // GET Order/Balance/userid
        [HttpGet("Balance/{id}")]
        public async Task<IActionResult> GetTellerBalance([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var teller = _tellerRepository.GetAll().Where(t => t.TellerId == id).FirstOrDefault();
            //.Select(t => t.Transactions.Where(c => c.Type == "Credit").Select(a => a.Amount).Sum()
            //        - t.Transactions.Where(c => c.Type == "Debit").Select(a => a.Amount).Sum());
            decimal bal = 0;
            if (teller.Transactions.Count > 0)
            {
                bal = teller.Transactions.Where(c => c.Type == "Debit" && c.TellerId == teller.TellerId && c.NominalId == teller.NominalId).Select(a => a.Amount).Sum()
                          - teller.Transactions.Where(c => c.Type == "Credit" && c.TellerId == teller.TellerId && c.NominalId == teller.NominalId).Select(a => a.Amount).Sum();
            }

            return Ok(bal);
        }

        // GET Report/Daybook
        [HttpGet("Summary/{id}/{date}")]
        public async Task<IActionResult> GetTellersDaybook([FromRoute] string id, DateTime date)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var now = date.Date;
            var sum = _tellerRepository.GetAll().Where(t => t.Id.Equals(id)).FirstOrDefault();
            if (sum == null) return BadRequest("Teller Id Doesn't Exist");
            var open = sum.Transactions.Where(c => c.Type == "Debit" && c.NominalId == sum.NominalId && c.TellerId == sum.TellerId && c.Date.Date < now).Select(a => a.Amount).Sum()
                                    - sum.Transactions.Where(c => c.Type == "Credit" && c.NominalId == sum.NominalId && c.TellerId == sum.TellerId && c.Date.Date < now).Select(a => a.Amount).Sum();
            var cash = new
            {
                Opening = open,
                Credit = sum.Transactions.Where(c => c.Type == "Credit" && c.NominalId == sum.NominalId && c.TellerId == sum.TellerId && c.Date.Date == now).Select(a => a.Amount).Sum(),
                Debit = sum.Transactions.Where(c => c.Type == "Debit" && c.NominalId == sum.NominalId && c.TellerId == sum.TellerId && c.Date.Date == now).Select(a => a.Amount).Sum(),
                Balance = (sum.Transactions.Where(c => c.Type == "Debit" && c.NominalId == sum.NominalId && c.TellerId == sum.TellerId && c.Date.Date == now).Select(a => a.Amount).Sum()
                                    - sum.Transactions.Where(c => c.Type == "Credit" && c.NominalId == sum.NominalId && c.TellerId == sum.TellerId && c.Date.Date == now).Select(a => a.Amount).Sum())
                                    + open
            };
            var trans = _transactionRepository.GetAll().Where(t => t.TellerId == sum.TellerId && t.NominalId == sum.NominalId && t.Date.Date == date.Date);

            return Ok(new { cash, trans });
        }
        
        // POST Teller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Teller value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var app = _tellerRepository.Query().Any(u => u.Id.Equals(value.Id));
            if (app) return BadRequest("User is Already a Valid Teller");

            await _tellerRepository.InsertAsync(value);

            var teller = _tellerRepository.GetAll().Where(i => i.Id == value.Id).Select(t => new {
                t.Id,
                t.Nominal.Code,
                Name = t.Nominal.Description,
                User = t.AppUser.UserName,
                NoOfTrans = t.Transactions.Where(c => c.UserId == t.Id).Count()
            }).ToList();

            return Created($"teller/{value.Id}", teller);
        }

        // POST Teller/transfer
        [HttpPost("Transfer")]
        public async Task<IActionResult> PostTransfer([FromBody] Transaction value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var from = _tellerRepository.Query().Where(i => i.Id == value.UserId).FirstOrDefault();
            if (from == null) return BadRequest("You Are not allowed to Make Transfer");

            var to = _tellerRepository.Query().Where(i => i.TellerId == value.TellerId).FirstOrDefault();
            if (to == null) return BadRequest($"There is no valid teller with Id {value.TellerId}");
            value.NominalId = to.NominalId; value.Type = "Debit"; value.Source = "Teller Transfer";
            value.Code = await _sequenceRepository.GetCode("Transaction");
            var tell = new Transaction()
            {
                Code = value.Code, Amount = value.Amount, Method = value.Method, Source = "Teller Transfer",
                Type = "Credit", NominalId = from.NominalId, TellerId = from.TellerId,
                Reference = value.Reference, UserId = value.UserId, Date = value.Date
            };
            await _transactionRepository.InsertAsync(value);
            await _transactionRepository.InsertAsync(tell);

            return Created($"transaction/{value.TransactionId}", value);
        }

        // POST Teller/voucher
        [HttpPost("Voucher")]
        public async Task<IActionResult> PostVoucher([FromBody] Transaction value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var from = _tellerRepository.Query().Where(i => i.Id == value.UserId).FirstOrDefault();
            if (from == null) return BadRequest("You Are not allowed to Make Transfer");

            var to = _nominalRepository.Query().Where(i => i.NominalId == value.NominalId).FirstOrDefault();
            if (to == null) return BadRequest($"There is no Nominal with Id {value.NominalId}");
            value.NominalId = to.NominalId; value.Type = "Debit"; value.Source = "Teller Voucher";
            value.Code = await _sequenceRepository.GetCode("Transaction");
            var tell = new Transaction()
            {
                Code = value.Code, Amount = value.Amount, Method = value.Method, Source = "Teller Voucher",
                Type = "Credit", NominalId = from.NominalId,TellerId = from.TellerId,
                Reference = value.Reference, UserId = value.UserId, Date = value.Date
            };
            await _transactionRepository.InsertAsync(value);
            await _transactionRepository.InsertAsync(tell);

            return Created($"transaction/{value.TransactionId}", value);
        }
        
        // PUT api/Teller
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Teller value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.TellerId) return BadRequest();

            await _tellerRepository.UpdateAsync(value);

            return Ok(value);
        }

        // DELETE api/Teller
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var teller = await _tellerRepository.DeleteAsync(id);

            return Ok(teller);
        }

        public class Days
        {
            [Required]
            public DateTime Date { get; set; }
            [Required]
            public int TellerId { get; set; }
        }

    }
}