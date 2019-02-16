using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LoanApi.Models;
using LoanApi.Repository;
using System.Linq;

namespace LoanApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class NominalController : ControllerBase
    {
        private readonly ITellerRepository _tellerRepository;
        private readonly INominalRepository _nominalRepository;
        private readonly ISequenceRepository _sequenceRepository;
        private readonly ITransactionRepository _transactionRepository;

        public NominalController(ITellerRepository tellerRepository, ISequenceRepository sequenceRepository,
            INominalRepository nominalRepository, ITransactionRepository transactionRepository)
        {
            _tellerRepository = tellerRepository;
            _nominalRepository = nominalRepository;
            _sequenceRepository = sequenceRepository;
            _transactionRepository = transactionRepository;
        }

        // GET api/Nominal
        [HttpGet]
        public IActionResult Get()
        {
            var nominal = _nominalRepository.Query();

            return Ok(nominal);
        }

        // GET api/Nominal
        [HttpGet("Main")]
        public IActionResult GetMain()
        {
            var nominal = _nominalRepository.GetMain();

            return Ok(nominal);
        }

        // GET api/Nominal
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var nominal = await _nominalRepository.GetAsync(id);

            if (nominal != null)
            {
                return Ok(nominal);
            }
            else
                return BadRequest();
        }
        
        // GET Nominal
        [HttpGet("balancetype/{type}")]
        public async Task<IActionResult> GetByBalanceType([FromRoute] string type)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var nominal = _nominalRepository.Query().Where(b => b.BalanceType.ToLower() == type.ToLower());

            if (nominal.Count() >= 0)
            {
                return Ok(nominal);
            }
            else
                return BadRequest();
        }
        
        // GET Order/Balance/userid
        [HttpGet("Balance/{id}")]
        public async Task<IActionResult> GetTellerBalance([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var teller = _nominalRepository.GetAll().Where(t => t.NominalId == id).FirstOrDefault();
            //.Select(t => t.Transactions.Where(c => c.Type == "Credit").Select(a => a.Amount).Sum()
            //        - t.Transactions.Where(c => c.Type == "Debit").Select(a => a.Amount).Sum());
            decimal bal = 0;
            if (teller.Transactions.Count > 0)
            {
                bal = teller.Transactions.Where(c => c.Type == "Debit" && c.NominalId == teller.NominalId).Select(a => a.Amount).Sum()
                          - teller.Transactions.Where(c => c.Type == "Credit" && c.NominalId == teller.NominalId).Select(a => a.Amount).Sum();
            }

            return Ok(bal);
        }

        // GET Nominal
        [HttpGet("gltype/{type}")]
        public async Task<IActionResult> GetByGlType([FromRoute] string type)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var nominal = _nominalRepository.Query().Where(b => b.GLType.ToLower() == type.ToLower());

            if (nominal.Count() >= 0)
            {
                return Ok(nominal);
            }
            else
                return BadRequest();
        }

        // POST api/Nominal
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Nominal value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            value.Code = await _sequenceRepository.GetCode("Nominal");

            await _nominalRepository.InsertAsync(value);

            return Created($"nominal/{value.NominalId}", value);
        }

        // POST Teller/transfer
        [HttpPost("Transfer")]
        public async Task<IActionResult> PostTransfer([FromBody] Transaction value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var from = _tellerRepository.Query().Where(i => i.TellerId == value.TellerId).FirstOrDefault();
            if (from == null) return BadRequest("You Are not allowed to Make Transfer");

            var to = _nominalRepository.Query().Where(i => i.NominalId == value.NominalId).FirstOrDefault();
            if (to == null) return BadRequest($"There is no valid Nominal with Id {value.NominalId}");
            value.NominalId = to.NominalId; value.Type = "Debit"; value.Source = "Financial Transfer";
            value.TellerId = null; value.Code = await _sequenceRepository.GetCode("Transaction");
            var tell = new Transaction()
            {
                Code = value.Code, Amount = value.Amount, Method = value.Method,
                Source = "Financial Transfer", Type = "Credit", NominalId = from.NominalId,
                TellerId = from.TellerId, Reference = value.Reference,
                UserId = value.UserId, Date = value.Date
            };
            await _transactionRepository.InsertAsync(value);
            await _transactionRepository.InsertAsync(tell);

            return Created($"transaction/{value.TransactionId}", value);
        }

        // POST Teller/Payment
        [HttpPost("Payment")]
        public async Task<IActionResult> PostPayment([FromBody] Transaction value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var from = _nominalRepository.Query().Where(i => i.NominalId == value.NominalId).FirstOrDefault();
            if (from == null) return BadRequest("Select a valid Funding Source to Make Payment");

            var to = _nominalRepository.Query().Where(i => i.NominalId == value.TellerId).FirstOrDefault();
            if (to == null) return BadRequest($"There is no Nominal with Id {value.TellerId}");
            value.NominalId = to.NominalId; value.Type = "Debit"; value.Source = "Financial";
            value.Code = await _sequenceRepository.GetCode("Transaction"); value.TellerId = null;
            var tell = new Transaction()
            {
                Code = value.Code, Amount = value.Amount, Method = value.Method,
                Source = value.Source, Type = "Credit", NominalId = from.NominalId,
                Reference = value.Reference, UserId = value.UserId, Date = value.Date
            };
            await _transactionRepository.InsertAsync(value);
            await _transactionRepository.InsertAsync(tell);

            return Created($"transaction/{value.TransactionId}", value);
        }

        // POST Teller/Payment
        [HttpPost("Disburment")]
        public async Task<IActionResult> PostDisburment([FromBody] Transaction value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var from = _nominalRepository.Query().Where(i => i.NominalId == value.NominalId).FirstOrDefault();
            if (from == null) return BadRequest("Select a valid Funding Source to Make Payment");

            var to = _tellerRepository.Query().Where(i => i.TellerId == value.TellerId).FirstOrDefault();
            if (to == null) return BadRequest($"There is no Teller with Id {value.TellerId}");
            value.NominalId = to.NominalId; value.Type = "Debit"; value.Source = "Financial";
            value.Code = await _sequenceRepository.GetCode("Transaction"); value.TellerId = value.TellerId;
            var nom = new Transaction()
            {
                Code = value.Code, Amount = value.Amount, Method = value.Method,
                Source = value.Source, Type = "Credit", NominalId = from.NominalId,
                TellerId = value.TellerId, Reference = value.Reference, UserId = value.UserId, Date = value.Date
            };
            await _transactionRepository.InsertAsync(value);
            await _transactionRepository.InsertAsync(nom);

            return Created($"transaction/{value.TransactionId}", value);
        }

        // PUT api/Nominal/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Nominal value, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != value.NominalId) return BadRequest();

            await _nominalRepository.UpdateAsync(value);

            return Ok(value);
        }

        // DELETE api/Nominal
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var nominal = await _nominalRepository.DeleteAsync(id);

            return Ok(nominal);
        }

    }
}