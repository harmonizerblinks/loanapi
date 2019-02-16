using LoanApi.Models;

namespace LoanApi.Repository
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
