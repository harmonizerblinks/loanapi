using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LoanApi.Models;
using LoanApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Controllers
{
    //[Route("[controller]")]
    //[AllowAnonymous]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ITellerRepository _tellerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly INominalRepository _nominalRepository;

        public ReportController(ITellerRepository tellerRepository, ITransactionRepository transactionRepository,
            INominalRepository nominalRepository)
        {
            _tellerRepository = tellerRepository;
            _nominalRepository = nominalRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost("Report")]
        public IActionResult Generate([FromBody] Reports model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var trialbalance = new List<TrialModel>();
            var trans = _nominalRepository.GetAll().ToList();
            for (int i = 0; i < trans.Count; i++)
            {
                var debit = _transactionRepository.Query().Where(a => a.NominalId == trans[i].NominalId && a.Date.Date >= model.Start.Date &&
                    a.Type.ToLower().Contains("debit") && a.Date < model.End).Select(a => a.Amount).Sum();
                var credit = _transactionRepository.Query().Where(a => a.NominalId == trans[i].NominalId && a.Date.Date >= model.Start.Date &&
                    a.Type.ToLower().Contains("credit") && a.Date < model.End).Select(a => a.Amount).Sum();
                var d = ((_transactionRepository.Query().Where(a =>
                                         a.NominalId == trans[i].NominalId && a.Type.ToLower().Contains("debit") &&
                                         a.Date.Date < model.Start.Date).Select(a => a.Amount)).Sum());
                var c = ((_transactionRepository.Query().Where(a =>
                                         a.NominalId == trans[i].NominalId && a.Type.ToLower().Contains("credit") &&
                                         a.Date.Date < model.Start.Date).Select(a => a.Amount)).Sum());
                var openingbal = _transactionRepository.Query().Where(a =>
                                         a.NominalId == trans[i].NominalId && a.Type.ToLower().Contains("debit") &&
                                         a.Date < model.Start).Select(a => a.Amount).Sum()
                                 - _transactionRepository.Query().Where(a =>
                                         a.NominalId == trans[i].NominalId && a.Type.ToLower().Contains("credit") &&
                                         a.Date < model.Start).Select(a => a.Amount).Sum();

                var o = ((d > c) ? (d - c) + " Dr" : (c - d) + " Cr");
                if (d == c) o = "0.00";

                trialbalance.Add(
                    new TrialModel
                    {
                        Code = trans[i].Code,
                        BalanceType = trans[i].BalanceType,
                        Name = trans[i].Description,
                        OpeningBalance = o,
                        //OpeningBalance = (openingbal == 0 ? openingbal + "" : (openingbal < 0 ? openingbal * (-1) + "" : openingbal + "")),
                        Debits = String.Format("{0:0.00}", debit),
                        Credits = String.Format("{0:0.00}", credit),
                        ClosingBalance = ((debit > (credit - openingbal)) ? (debit - (credit - openingbal)) + " Dr" : ((credit - openingbal) - debit) + " Cr")
                        //ClosingBalance = ((debit > (credit + openingbal)) ? (debit - credit) + " Dr" : (credit - debit) + " Cr")
                    });

            }
            return Ok(trialbalance);
        }

        // GET Report
        [HttpGet("Cashbook")]
        public async Task<IActionResult> GetCashbook()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //var book = new List<Cashbook>();
            var cashbook = _nominalRepository.Query().Where(l => l.GLType.ToLower() == "cashbook").ToList();
            for (int i = 0; i < cashbook.Count; i++)
            {
                var bal = _transactionRepository.Query().Where(c => c.Type == "Debit" && c.NominalId == cashbook[i].NominalId).Select(a => a.Amount).Sum()
                            - _transactionRepository.Query().Where(c => c.Type == "Credit" && c.NominalId == cashbook[i].NominalId).Select(a => a.Amount).Sum();
                cashbook[i].Balance = bal;
                //book.Add(
                //    new Cashbook
                //    {
                //        Id =cashbook[i].NominalId,
                //        Code = cashbook[i].Code,
                //        Name = cashbook[i].Description,
                //        Gltype = cashbook[i].GLType,
                //        BalanceType = cashbook[i].BalanceType,
                //        Status = cashbook[i].Status,
                //        Balance = (bal == 0 ? bal + "" : (bal < 0 ? bal * (-1) + "" : bal + ""))
                //    });
            };

            return Ok(cashbook);
        }

        // GET Report/Summary/date
        [HttpGet("Summary")]
        public async Task<IActionResult> GetTellersSummary()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var summary = new List<Summary>();
            var teller = _tellerRepository.GetAll().ToList();
            for (int i = 0; i < teller.Count; i++)
            {
                var Count = _transactionRepository.Query().Where(c => c.TellerId == teller[i].TellerId && c.NominalId == teller[i].NominalId).Count();
                //var Opening = _transactionRepository.Query().Where(c => c.Type == "Debit" && c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId && c.Date.Date < teller[i].Date.Date)
                //                    .Select(a => a.Amount).Sum() - _transactionRepository.Query().Where(c => c.Type == "Credit" &&
                //                    c.NominalId == teller[i].NominalId && c.Date.Date < teller[i].Date.Date).Select(a => a.Amount).Sum();
                var Credit = _transactionRepository.Query().Where(c => c.Type == "Credit" && c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId).Select(a => a.Amount).Sum();
                var Debit = _transactionRepository.Query().Where(c => c.Type == "Debit" && c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId).Select(a => a.Amount).Sum();
                var Balance = _transactionRepository.Query().Where(c => c.Type == "Debit" && c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId)
                                    .Select(a => a.Amount).Sum() - _transactionRepository.Query().Where(c => c.Type == "Credit" &&
                                    c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId).Select(a => a.Amount).Sum();
                summary.Add(new Summary
                {
                    Id = teller[i].TellerId,
                    Code = teller[i].Nominal.Code,
                    Name = teller[i].Nominal.Description,
                    User = teller[i].AppUser.UserName,
                    NoOfTrans = Count,
                    //Opening = Opening,
                    Credit = Credit,
                    Debit = Debit,
                    Balance = Balance
                });
            }

            return Ok(summary);
        }

        // GET Report/Summary/date
        [HttpGet("Summary/{date}")]
        public async Task<IActionResult> GetTellersSummary([FromRoute] DateTime date)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var now = date.Date; var summary = new List<Summary>();
            var teller = _tellerRepository.GetAll().ToList();
            for (int i = 0; i < teller.Count; i++)
            {
                var Count = _transactionRepository.Query().Where(c => c.TellerId == teller[i].TellerId && c.NominalId == teller[i].NominalId && c.Date.Date == now).Count();
                var Opening = _transactionRepository.Query().Where(c => c.Type == "Debit" && c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId && c.Date.Date < now)
                                    .Select(a => a.Amount).Sum() - _transactionRepository.Query().Where(c => c.Type == "Credit" &&
                                    c.NominalId == teller[i].NominalId && c.Date.Date < now).Select(a => a.Amount).Sum();
                var Credit = _transactionRepository.Query().Where(c => c.Type == "Credit" && c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId && c.Date.Date == now).Select(a => a.Amount).Sum();
                var Debit = _transactionRepository.Query().Where(c => c.Type == "Debit" && c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId && c.Date.Date == now).Select(a => a.Amount).Sum();
                var Balance = _transactionRepository.Query().Where(c => c.Type == "Debit" && c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId && c.Date.Date == now)
                                    .Select(a => a.Amount).Sum() - _transactionRepository.Query().Where(c => c.Type == "Credit" &&
                                    c.NominalId == teller[i].NominalId && c.TellerId == teller[i].TellerId && c.Date.Date < now).Select(a => a.Amount).Sum() + Opening;
                summary.Add(new Summary
                {
                    Id = teller[i].TellerId,
                    Code = teller[i].Nominal.Code,
                    Name = teller[i].Nominal.Description,
                    User = teller[i].AppUser.UserName,
                    NoOfTrans = Count,
                    Opening = Opening,
                    Credit = Credit,
                    Debit = Debit,
                    Balance = Balance
                });
            }

            return Ok(summary);
        }

        // GET Report/Daybook
        [HttpPost("DayBook")]
        public async Task<IActionResult> GetTellersDaybook([FromBody] Daybook data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var now = data.Date.Date;
            var sum = _tellerRepository.GetAll().Where(t => t.TellerId.Equals(data.TellerId)).FirstOrDefault();
            if (sum == null) return BadRequest("Teller Id Doesn't Exist");
            var open = sum.Transactions.Where(c => c.Type == "Debit" && c.NominalId == sum.NominalId && c.TellerId == sum.TellerId && c.Date < now).Select(a => a.Amount).Sum()
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
            var trans = _transactionRepository.Query().Where(t => t.TellerId == sum.TellerId && t.NominalId == sum.NominalId && t.Date.Date == data.Date.Date);

            return Ok(new { cash, trans });
        }

        // GET Report/Enquiry
        [HttpPost("Enquiry")]
        public async Task<IActionResult> GetTNominalEnquiry([FromBody] Reports data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (data.Start > data.End) return BadRequest("End Date must be Greater than Start date");
            var sum = _nominalRepository.GetAll().Where(t => t.NominalId.Equals(data.Id)).FirstOrDefault();
            if (sum == null) return BadRequest("Select A Valid Nominal Id");
            var cash = new
            {
                Opening = sum.Transactions.Where(c => c.Type == "Debit" && c.NominalId == sum.NominalId &&
                            c.Date.Date < data.Start.Date).Select(a => a.Amount).Sum()
                                - sum.Transactions.Where(c => c.Type == "Credit" && c.NominalId == sum.NominalId &&
                                    c.Date.Date < data.Start.Date).Select(a => a.Amount).Sum(),
                Credit = sum.Transactions.Where(c => c.Type == "Credit" && c.NominalId == sum.NominalId && c.Date.Date >=
                                    data.Start.Date && c.Date.Date <= data.End.Date).Select(a => a.Amount).Sum(),
                Debit = sum.Transactions.Where(c => c.Type == "Debit" && c.NominalId == sum.NominalId && c.Date.Date >=
                                    data.Start.Date && c.Date.Date <= data.End.Date).Select(a => a.Amount).Sum(),
                Balance = sum.Transactions.Where(c => c.Type == "Debit" && c.NominalId == sum.NominalId &&
                            c.Date.Date >= data.Start.Date && c.Date.Date <= data.End.Date).Select(a => a.Amount).Sum()
                                - sum.Transactions.Where(c => c.Type == "Credit" && c.NominalId == sum.NominalId && c.Date.Date >=
                                    data.Start.Date && c.Date.Date <= data.End.Date).Select(a => a.Amount).Sum(),
            };
            var trans = _transactionRepository.Query().Where(n => n.NominalId == sum.NominalId && n.Date.Date >= data.Start.Date
                           && n.Date.Date <= data.End.Date);

            return Ok(new { cash, trans });
        }

        [HttpGet("Enquirys")]
        public IActionResult GenerateNominal([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var codes = new List<Codes>();
            var main = _nominalRepository.Query().Select(t => t.GLType).Distinct().ToList();

            for (int g = 0; g < main.Count; g++)
            {
                var nom = _nominalRepository.Query().Where(a => a.GLType.ToLower() == main[g].ToLower()).ToList();

                codes.Add(new Codes { Name = main[g], Nominal = nom });
            }
            return Ok(codes);
        }

        [HttpPost("Income")]
        public IActionResult Generate([FromBody] Report model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var income = new List<Income>();
            var main = _nominalRepository.GetMain().Where(u => u.Name.ToLower() == "income" || u.Name.ToLower() == "expense").ToList();
            //var nominalCodes = (_context.GeneralLedgerCodes.Where(a => a.BranchId == model.Id && a.CurrencyId == curr.CurrencyId)
            //    .Select(a => a.GeneralLedgerCodeId)).Distinct().ToList();
            decimal tdb = 0; decimal tcd = 0; decimal topd = 0; decimal topc = 0;

            for (int g = 0; g < main.Count; g++)
            {
                var trial = new List<TrialModel>();
                decimal db = 0; decimal cd = 0; decimal opd = 0; decimal opc = 0;

                var nom = (_nominalRepository.Query().Where(a => a.MainNominalId == main[g].MainNominalId)
                                    .Select(a => a.NominalId)).ToList();

                for (int i = 0; i < nom.Count; i++)
                {
                    var debit = (_transactionRepository.Query().Where(a =>
                        a.NominalId == nom[i] && a.Date.Date >= model.From.Date &&
                        a.Type.ToLower().Contains("debit") && a.Date.Date <= model.To.Date).Select(a => a.Amount)).Sum();
                    var credit = (_transactionRepository.Query().Where(a =>
                        a.NominalId == nom[i] && a.Date.Date >= model.From &&
                        a.Type.ToLower().Contains("credit") && a.Date.Date <= model.To.Date).Select(a => a.Amount)).Sum();
                    var d = ((_transactionRepository.Query().Where(a =>
                                             a.NominalId == nom[i] && a.Type.ToLower().Contains("debit") &&
                                             a.Date.Date < model.From.Date).Select(a => a.Amount)).Sum());
                    var c = ((_transactionRepository.Query().Where(a =>
                                             a.NominalId == nom[i] && a.Type.ToLower().Contains("credit") &&
                                             a.Date.Date < model.From.Date).Select(a => a.Amount)).Sum());
                    var openingbal = ((_transactionRepository.Query().Where(a =>
                                             a.NominalId == nom[i] && a.Type.ToLower().Contains("debit") &&
                                             a.Date.Date < model.From.Date).Select(a => a.Amount)).Sum())
                                     - ((_transactionRepository.Query().Where(a =>
                                             a.NominalId == nom[i] && a.Type.ToLower().Contains("credit") &&
                                             a.Date.Date < model.To.Date).Select(a => a.Amount)).Sum());

                    db += debit; cd += credit;
                    opd += d; opc += c;
                    var o = ((d > c) ? (d - c) + " Dr" : (c - d) + " Cr");
                    if (d == c) o = "0.00";

                    trial.Add(
                        new TrialModel
                        {
                            OpeningBalance = o,
                            // OpeningBalance = (openingbal == 0 ? openingbal + "" : (openingbal < 0 ? openingbal * (-1) + "" : openingbal + "")),
                            Debits = String.Format("{0:0.00}", debit),
                            Credits = String.Format("{0:0.00}", credit),
                            Name = ((_nominalRepository.Query().Where(a => a.NominalId == nom[i])
                                .Select(a => a.Code + " - " + a.Description)).FirstOrDefault()),
                            ClosingBalance = ((debit > (credit - openingbal)) ? (debit - (credit - openingbal)) + " Dr" : ((credit - openingbal) - debit) + " Cr")
                        });

                }
                //var t = ((opd > opc) ? (opd - opc) + " Dr" : (opd - opc) + " Cr");
                //if (opd == opc) t = "0.00";
                var total = new Total()
                {
                    Description = "Total",
                    Opening = ((opd > opc) ? (opd - opc) + " Dr" : (opd - opc) + " Cr"),
                    Debit = String.Format("{0:0.00}", db),
                    Credit = String.Format("{0:0.00}", cd),
                    Closing = ((db > cd) ? (db - cd) + " Dr" : (cd - db) + " Cr")
                };

                income.Add(new Income { Name = main[g].Code + " " + main[g].Name, Trans = trial, Sum = total });

            }
            var profit = new Total() { };
            return Ok(income);
        }

        public class Report
        {
            [Required]
            public DateTime From { get; set; }
            [Required]
            public DateTime To { get; set; }
            public int NominalId { get; set; }
        }
        public class Daybook
        {
            [Required]
            public DateTime Date { get; set; }
            [Required]
            public int TellerId { get; set; }
        }

        public class Summary
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string User { get; set; }
            public int NoOfTrans { get; set; }
            public decimal Opening { get; set; }
            public decimal Credit { get; set; }
            public decimal Debit { get; set; }
            public decimal Balance { get; set; }
        }

        public class Cashbook
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Gltype { get; set; }
            public string BalanceType { get; set; }
            public string Status { get; set; }
            public string Balance { get; set; }
        }

        public class Reports
        {
            public int? Id { get; set; }
            [Required]
            public DateTime Start { get; set; }
            [Required]
            public DateTime End { get; set; }
        }
        public class TrialModel
        {
            public string Code { get; set; }
            public string BalanceType { get; set; }
            public string Name { get; set; }
            public string OpeningBalance { get; set; }
            public string Debits { get; set; }
            public string Credits { get; set; }
            public string ClosingBalance { get; set; }
        }

        public class Total
        {
            public string Description { get; set; }
            public string Opening { get; set; }
            public string Debit { get; set; }
            public string Credit { get; set; }
            public string Closing { get; set; }
        }

        public class Income
        {
            public string Name { get; set; }
            public List<TrialModel> Trans { get; set; }
            public Total Sum { get; set; }
        }

        public class Codes
        {
            public string Name { get; set; }
            public List<Nominal> Nominal { get; set; }
        }
    }
}