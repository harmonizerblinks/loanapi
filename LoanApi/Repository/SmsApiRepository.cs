using LoanApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Repository
{
    public class SmsApiRepository : GenericRepository<SmsApi>, ISmsApiRepository
    {
        public SmsApiRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
