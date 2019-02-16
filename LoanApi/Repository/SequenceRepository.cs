using LoanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LoanApi.Repository
{
    public class SequenceRepository : GenericRepository<Sequence>, ISequenceRepository
    {
        public SequenceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetCode(string type)
        {
            var sequence = await _dbContext.Sequence.FirstOrDefaultAsync(a => a.Name.ToLower() == type.ToLower());

            if (sequence != null)
            {
                string transcodenum = (sequence.Counter).ToString();
                int length = sequence.Length;
                int padnum = length - transcodenum.Length;
                string number = transcodenum.PadLeft(padnum + 1, '0');
                string code = sequence.Prefix + number;

                sequence.Counter += 1;
                _dbContext.Entry(sequence).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return code;
            }
            return null;
        }

        public async Task<string> GetCode(int id)
        {
            var sequence = await _dbContext.Sequence.FirstOrDefaultAsync(a => a.SequenceId == id);

            if (sequence != null)
            {
                string transcodenum = (sequence.Counter).ToString();
                int length = sequence.Length;
                int padnum = length - transcodenum.Length;
                string number = transcodenum.PadLeft(padnum + 1, '0');
                string code = sequence.Prefix + number;

                sequence.Counter += 1;
                _dbContext.Entry(sequence).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return code;
            }
            return null;
        }


    }
}
