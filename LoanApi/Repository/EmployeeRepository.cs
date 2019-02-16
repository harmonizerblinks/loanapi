using LoanApi.Models;

namespace LoanApi.Repository
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
