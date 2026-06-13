using Microsoft.EntityFrameworkCore;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Data;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Services
{
    public class RadnikService : IRadnikService
    {
        private readonly PvcStolarijaDbContext _context;
        public RadnikService(PvcStolarijaDbContext context) => _context = context;

        public async Task<IEnumerable<Radnik>> GetAllAsync() =>
            await _context.Radnici.ToListAsync();

        public async Task<Radnik?> GetByIdAsync(int id) =>
            await _context.Radnici.FindAsync(id);

        public async Task AddAsync(Radnik radnik)
        {
            _context.Radnici.Add(radnik);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Radnik radnik)
        {
            _context.Radnici.Update(radnik);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var radnik = await _context.Radnici.FindAsync(id);
            if (radnik != null)
            {
                _context.Radnici.Remove(radnik);
                await _context.SaveChangesAsync();
            }
        }
    }
}
