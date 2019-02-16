using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LoanApi.Models;
using LoanApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LoanApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly ICotRepository _cotRepository;
        private readonly ISmsRepository _smsRepository;
        private readonly ISmsApiRepository _smsapiRepository;
        private readonly IChargeRepository _chargeRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly ITransitRepository _transitRepository;
        private readonly IChequeRepository _chequeRepository;
        private readonly INominalRepository _nominalRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISequenceRepository _sequenceRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IAccountTypeRepository _accounttypeRepository;
        private readonly ITransactionRepository _transactionRepository;

        public ProcessController(IAccountRepository accountRepository, ICustomerRepository customerRepository,
            ISequenceRepository sequenceRepository, ITellerRepository tellerRepository, ICotRepository cotRepository,
            IChequeRepository chequeRepository, INominalRepository nominalRepository, ISmsRepository smsRepository,
            IAccountTypeRepository accounttypeRepository, ILocationRepository locationRepository, 
            ITransactionRepository transactionRepository, IChargeRepository chargeRepository, 
            ISmsApiRepository smsapiRepository, ITransitRepository transitRepository)
        {
            _cotRepository = cotRepository;
            _smsRepository = smsRepository;
            _chequeRepository = chequeRepository;
            _chargeRepository = chargeRepository;
            _tellerRepository = tellerRepository;
            _transitRepository = transitRepository;
            _accountRepository = accountRepository;
            _nominalRepository = nominalRepository;
            _customerRepository = customerRepository;
            _locationRepository = locationRepository;
            _sequenceRepository = sequenceRepository;
            _accounttypeRepository = accounttypeRepository;
            _transactionRepository = transactionRepository;
        }

        // POST api/Account
        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> PostAccountStatus([FromBody] Change value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var account = await _accountRepository.GetAsync(value.AccountId);
            if (account == null) return BadRequest(new { Message = "Select a Valid Account" });
            account.Status = value.Status;
            account.MDate = value.Date.Date; account.MUserId = value.UserId;

            await _accountRepository.UpdateAsync(account);

            return Ok(value);
        }

        // POST api/Account
        [HttpPost("Charge")]
        public async Task<IActionResult> PostCharge([FromBody] Charges value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var account = _accountRepository.GetAll(value.AccountId).FirstOrDefault();
            if (account == null) return BadRequest(new { Message = "Select a Valid Account" });
            var cots = _cotRepository.GetAll().Where(i=>i.CotId == account.AccountType.CotId).FirstOrDefault();
            if (cots == null) return BadRequest(new { Message = "You are not Authorized to Post Transaction" });

            account.Balance -= value.Amount;
            var code = await _sequenceRepository.GetCode("Transaction");
            var dep = new Transaction()
            {
                Code = code, Source = "Charge", Type = "Credit", Amount = value.Amount,
                Method = "Cash", AccountId = value.AccountId, NominalId = account.AccountType.NominalId,
                Reference = value.Reference, UserId = value.UserId, Date = value.Date.Date
            };
            var cot = new Transaction()
            {
                Code = code, Source = "Nominal", Type = "Debit", Amount = value.Amount,
                Method = dep.Method, AccountId = value.AccountId, NominalId = cots.NominalId,
                Reference = value.Reference, UserId = value.UserId, Date = value.Date.Date
            };
            var charge = new Charge() { AccountId = account.AccountId, Amount = value.Amount, CotId = cots.CotId,
                Method=dep.Method, Reference=value.Reference, Date=value.Date, UserId=value.UserId };
            
            await _transactionRepository.InsertAsync(dep);
            await _transactionRepository.InsertAsync(cot);
            await _accountRepository.UpdateAsync(account);
            await _chargeRepository.InsertAsync(charge);
            
            return Ok(value);
        }

        // POST api/Account
        [HttpPost("Deposit")]
        public async Task<IActionResult> PostDeposit([FromBody] Payment value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var account = _accountRepository.GetAll(value.AccountId).FirstOrDefault();
            if (account == null) return BadRequest(new { Message = "Select a Valid Account" });

            var teller = _tellerRepository.GetUser(value.UserId).FirstOrDefault();
            if (teller == null) return BadRequest(new { Message = "You are not Authorized to Post Transaction" });
            if (value.Method.ToLower() == "cheque" && value.Number == null) return BadRequest(new { Message = "Provide A valid Cheque number" });

            account.Balance += value.Amount; account.Days += value.Days;
            var code = await _sequenceRepository.GetCode("Transaction");
            var dep = new Transaction()
            {
                Code = code, TellerId = teller.TellerId, Source = "Teller",
                Type = "Credit", Amount = value.Amount, Method = value.Method,
                AccountId = value.AccountId, NominalId = account.AccountType.NominalId,
                Reference = value.Reference, UserId = value.UserId, Date = value.Date.Date
            };
            var tell = new Transaction()
            {
                Code = code, TellerId = teller.TellerId, Source = "Nominal",
                Type = "Debit", Amount = value.Amount, Method = value.Method,
                AccountId = value.AccountId, NominalId = teller.NominalId,
                Reference = value.Reference, UserId = value.UserId, Date = value.Date.Date
            };
            
            await _transactionRepository.InsertAsync(dep);
            await _transactionRepository.InsertAsync(tell);
            if(value.Method.ToLower() == "cheque" && value.Number != null)
            {
                var cheque = new Cheque() { TransactionId = dep.TransactionId, Number = value.Number,
                    AccountId = account.AccountId,  Date = value.Date.Date, UserId = value.UserId };
                await _chequeRepository.InsertAsync(cheque);
            }
            if(account.Alert == true)
            {
                var notify = new Sms()
                {
                    Type = "Deposit", Mobile = account.Customer.Mobile, Code = 1,AccountId = account.AccountId,
                    Message = $"Deposit Successfull, Your Account No: {account.Number} Has been Credited With Amount: {value.Amount}. New Balance: {account.Balance}. ",
                    Date=value.Date, UserId=value.UserId, Response = "Pending"
                };
                await _smsRepository.InsertAsync(notify);
            }
            await _accountRepository.UpdateAsync(account);

            return Ok(value);
        }

        // POST api/Account
        [HttpPost("Withdraw")]
        public async Task<IActionResult> PostWithdrawal([FromBody] Payment value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var account = _accountRepository.GetAll(value.AccountId).FirstOrDefault();
            if (account == null) return BadRequest(new { Message = "Select a Valid Account" });

            var teller = _tellerRepository.GetUser(value.UserId).FirstOrDefault();
            if (teller == null) return BadRequest(new { Message = "You are not Authorized to Post Transaction" });
            if (value.Method.ToLower() == "cheque" && value.Number == null) return BadRequest(new { Message = "Provide A valid Cheque number" });

            account.Balance -= value.Amount;
            if (account.AccountType.BaseType != "Loans" && account.Balance < 0) return BadRequest(new { Message = "Not Enough Money In Account" });
            if (account.AccountType.BaseType != "Savings") account.Days -= value.Days;

            var code = await _sequenceRepository.GetCode("Transaction");
            var dep = new Transaction()
            {
                Code = code, TellerId = teller.TellerId, Source = "Teller",
                Type = "Debit", Amount = value.Amount, Method = value.Method,
                AccountId = value.AccountId, NominalId = account.AccountType.NominalId,
                Reference = value.Reference, UserId = value.UserId,  Date = value.Date.Date
            };
            var tell = new Transaction()
            {
                Code = code, TellerId = teller.TellerId, Source = "Nominal",
                Type = "Credit", Amount = value.Amount, Method = value.Method,
                AccountId = value.AccountId, NominalId = teller.NominalId,
                Reference = value.Reference, UserId = value.UserId, Date = value.Date.Date
            };

            await _transactionRepository.InsertAsync(dep);
            await _transactionRepository.InsertAsync(tell);
            await _accountRepository.UpdateAsync(account);

            if (value.Method.ToLower() == "cheque" && value.Number != null)
            {
                var cheque = new Cheque()
                {
                    TransactionId = dep.TransactionId, Number = value.Number, AccountId = account.AccountId,
                    Date = value.Date, UserId = value.UserId
                };

            }
            if (account.Alert == true)
            {
                var notify = new Sms()
                {
                    Type = "Withdrawal", Mobile = account.Customer.Mobile, Code = 1, AccountId = account.AccountId,
                    Message = $"Withdrawal Successfull, Your Account No: {account.Number} Has been Debited With Amount: {value.Amount}. New Balance: {account.Balance}",
                    Date = value.Date, UserId = value.UserId, Response="Pending"
                };
                await _smsRepository.InsertAsync(notify);

            }
            
            return Ok(value);
        }


        // POST Teller/Receipt
        [HttpPost("Receipt")]
        public async Task<IActionResult> PostPayment([FromBody] Transaction value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var from = _transitRepository.Query().Where(i => i.Method.ToLower() == value.Method.ToLower()).FirstOrDefault();
            if (from == null) return BadRequest("There is no Transit with Selected Payment Method");

            var to = _nominalRepository.Query().Where(i => i.NominalId == value.NominalId).FirstOrDefault();
            if (to == null) return BadRequest($"There is no Nominal with Id {value.NominalId}");
            value.NominalId = to.NominalId; value.Type = "Debit"; value.Source = "Financial";
            value.Code = await _sequenceRepository.GetCode("Transaction");
            var tell = new Transaction()
            {
                Code = value.Code,
                Amount = value.Amount,
                Method = value.Method,
                Source = value.Source,
                Type = "Credit",
                NominalId = from.NominalId,
                Reference = value.Reference,
                UserId = value.UserId,
                Date = value.Date
            };
            await _transactionRepository.InsertAsync(value);
            await _transactionRepository.InsertAsync(tell);

            return Created($"transaction/{value.TransactionId}", value);
        }


        // GET api/Account
        [HttpGet("Transaction/{id}")]
        public async Task<IActionResult> GetAccountTransaction([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var trans = _transactionRepository.Query().Where(c => c.AccountId == id && c.Source != "Nominal")
                        .OrderByDescending(d => d.Date).OrderByDescending(d => d.TransactionId);

            if (trans != null)
            {
                return Ok(trans);
            }
            else
                return BadRequest();
        }

        // GET api/Account
        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //var trans = _transactionRepository.Query().Where(c => c.AccountId == id && c.Source != "Nominal")
            //            .OrderByDescending(d => d.Date).OrderByDescending(d => d.TransactionId);
            //var incomenom = _nominalRepository.GetAll().Where(n => n.MainNominal.Name.ToLower() == "income");
            //var income = _transactionRepository.GetAll().Where(c => c.Nominal.GLType == "income").Select(t => t.Amount).Sum();
            var dash = new
            {
                customers = _customerRepository.Query().Count(),
                female = _customerRepository.Query().Where(g => g.Gender.ToLower() == "female").Count(),
                male = _customerRepository.Query().Where(g => g.Gender.ToLower() == "male").Count(),
                account = _accountRepository.Query().Count(),
                savings = _accountRepository.GetAll().Where(g => g.AccountType.BaseType.ToLower() == "savings").Count(),
                loans = _accountRepository.GetAll().Where(g => g.AccountType.BaseType.ToLower() == "loans").Count(),
                trans = _transactionRepository.Query().Count(),
                income = _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "income" && c.Type == "Debit").Select(t => t.Amount).Sum() 
                        - _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "income" && c.Type == "Credit").Select(t => t.Amount).Sum(),
                expense = _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "expense" && c.Type == "Debit").Select(t => t.Amount).Sum()
                        - _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "expense" && c.Type == "Credit").Select(t => t.Amount).Sum(),
                liability = _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "liability" && c.Type == "Credit").Select(t => t.Amount).Sum() 
                        - _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "liability" && c.Type == "Debit").Select(t => t.Amount).Sum(),
                assert = _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "asset" && c.Type == "Credit").Select(t => t.Amount).Sum()
                        - _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "asset" && c.Type == "Debit").Select(t => t.Amount).Sum(),
                cashbook = _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "cashbook" && c.Type == "Debit").Select(t => t.Amount).Sum()
                        - _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "cashbook" && c.Type == "Credit").Select(t => t.Amount).Sum(),
                teller = _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "teller" && c.Type == "Debit").Select(t => t.Amount).Sum()
                        - _transactionRepository.GetAll().Where(c => c.Nominal.GLType.ToLower() == "teller" && c.Type == "Credit").Select(t => t.Amount).Sum(),
            };
            
            return Ok(dash);
        }
        
        // GET Sms
        [HttpGet("Sms")]
        public async Task<IActionResult> PostSms([FromRoute] SmsBoardcast sms)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var config = _smsapiRepository.Query().LastOrDefault(a => a.Status.ToLower().Contains("active") && a.Default == true);
            //var sms = await _smsRepository.GetAsync(id);
            var httpClient = new HttpClient();
            StringBuilder sb = new StringBuilder();
            sb.Append(config.Url).Append("?cmd=sendquickmsg&owneremail=Harmonizerblinks@gmail.com&subacct=")
                .Append(config.Username).Append("&subacctpwd=").Append(config.Password)
                .Append("&message=").Append(sms.Message).Append("&sender=").Append(config.SenderId)
                .Append("&sendto=").Append(sms.Mobile).Append("&msgtype=0");
            //sb.Append(config.Url).Append("?&username=").Append(config.Username)
            //    .Append("&password=").Append(config.Password).Append("&source=").Append(config.SenderId)
            //    .Append("&destination=").Append(sms.Mobile).Append("&message=").Append(sms.Message);
            var json = await httpClient.GetStringAsync(sb.ToString());
            //var smsresponse = JsonConvert.DeserializeObject<SmsResponse>(json);
            sms.Code = 0; sms.Response = json;

            sms = await _smsRepository.InsertBroadcast(sms);

            return Ok(sms);
        }

        public class Change : BaseModel
        {
            [Required]
            public int AccountId { get; set; }
            [Required]
            public string Status { get; set; }
        }

        public class Charges : BaseModel
        {
            [Required]
            public int AccountId { get; set; }
            [Required]
            public decimal Amount { get; set; }
            [Required]
            public string Reference { get; set; }
        }


        public class Payment : BaseModel
        {
            [Required]
            public int AccountId { get; set; }
            public int TellerId { get; set; }
            [Required]
            public int Days { get; set; }
            [Required]
            public decimal Amount { get; set; }
            public string Number { get; set; }
            [Required]
            public string Method { get; set; }
            [Required]
            public string Reference { get; set; }
        }

    }
}