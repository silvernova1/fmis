using fmis.Data;
using fmis.Models.ppmp;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fmis.Repositories
{
    public interface IAppExpenseRepository
    {
        Task<IEnumerable<AppExpense>> GetAllAsync();
        Task<AppExpense> GetByIdAsync(int id);
        Task<AppExpense> CreateAsync(AppExpense appExpense);
        Task<AppExpense> UpdateAsync(AppExpense appExpense);
        Task DeleteAsync(int id);
    }

    public class AppExpenseRepository : IAppExpenseRepository
    {
        private readonly MyDbContext _context;

        public AppExpenseRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppExpense>> GetAllAsync()
        {
            return await _context.AppExpense.ToListAsync();
        }

        public async Task<AppExpense> GetByIdAsync(int id)
        {
            return await _context.AppExpense.FindAsync(id);
        }

        public async Task<AppExpense> CreateAsync(AppExpense appExpense)
        {
            _context.AppExpense.Add(appExpense);
            await _context.SaveChangesAsync();
            return appExpense;
        }

        public async Task<AppExpense> UpdateAsync(AppExpense appExpense)
        {
            _context.Entry(appExpense).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return appExpense;
        }

        public async Task DeleteAsync(int id)
        {
            var appExpense = await _context.AppExpense.FindAsync(id);
            if (appExpense != null)
            {
                _context.AppExpense.Remove(appExpense);
                await _context.SaveChangesAsync();
            }
        }
    }
}
