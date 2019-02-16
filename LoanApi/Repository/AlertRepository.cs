using LoanApi.Models;

namespace LoanApi.Repository
{
    public class AlertRepository : GenericRepository<Alert>, IAlertRepository
    {
        public AlertRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
