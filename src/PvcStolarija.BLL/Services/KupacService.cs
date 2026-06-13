using Microsoft.EntityFrameworkCore;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Data;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Services
{
    public class KupacService : IKupacService
    {
        private readonly PvcStolarijaDbContext _context;
        public KupacService(PvcStolarijaDbContext context) => _context = context;

        public async Task<IEnumerable<Kupac>> GetAllAsync() =>
            await _context.Kupci.ToListAsync();

        public async Task<Kupac?> GetByIdAsync(int id) =>
            await _context.Kupci.FindAsync(id);

        public async Task AddAsync(Kupac kupac)
        {
            _context.Kupci.Add(kupac);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Kupac kupac)
        {
            _context.Kupci.Update(kupac);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var kupac = await _context.Kupci.FindAsync(id);
            if (kupac != null)
            {
                _context.Kupci.Remove(kupac);
                await _context.SaveChangesAsync();
            }
        }
    }
}
