using LoanApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Repository
{
    public class GarantorRepository : GenericRepository<Garantor>, IGarantorRepository
    {
        public GarantorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
