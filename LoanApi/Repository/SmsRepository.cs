using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Repository
{
    public class SmsRepository : GenericRepository<Sms>, ISmsRepository
    {
        public SmsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Sms> GetAll()
        {
            return _dbContext.Sms.Include(x => x.Account).Include(x => x.Customer).AsQueryable();
        }
        
        public IQueryable<SmsBoardcast> GetBroadcast()
        {
            return _dbContext.SmsBoardcast.AsQueryable();
        }

        public async Task<SmsBoardcast> InsertBroadcast(SmsBoardcast sms)
        {
            _dbContext.SmsBoardcast.Add(sms);
            await _dbContext.SaveChangesAsync();
            return sms;
        }
    }
}
