using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoanApi.Models;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using LoanApi.Repository;
using Microsoft.AspNetCore.SignalR;

namespace LoanApi.Services
{
    public class MyServices : IMyServices
    {
        //protected AppDbContext _context { get; set; }
        protected ISmsRepository _smsRepository;
        protected ISmsApiRepository _smsapiRepository;
        protected ISequenceRepository _sequenceRepository;
        //protected IHubContext<OrderHub> _order;
        
        public async Task<string> GetCode(string type)
        {
            //var sequence = await _context.Sequence.FirstOrDefaultAsync(a => a.Name.ToLower() == type.ToLower());
            var sequence = await _sequenceRepository.Query().FirstOrDefaultAsync(a => a.Name.ToLower() == type.ToLower());

            if (sequence != null)
            {
                string transcodenum = (sequence.Counter).ToString();
                int padnum = sequence.Length - transcodenum.Length;
                string number = transcodenum.PadLeft(padnum + 1, '0');
                string code = sequence.Prefix + number;

                sequence.Counter += 1;
                await _sequenceRepository.UpdateAsync(sequence);
                //_context.Entry(sequence).State = EntityState.Modified;
                //await _context.SaveChangesAsync();

                return code;
            }
            return null;
        }
        
    }
}
