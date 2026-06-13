using Microsoft.EntityFrameworkCore;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Data;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Services
{
    public class ProizvodService : IProizvodService
    {
        private readonly PvcStolarijaDbContext _context;
        public ProizvodService(PvcStolarijaDbContext context) => _context = context;

        public async Task<IEnumerable<Proizvod>> GetAllAsync() =>
            await _context.Proizvodi.Include(p => p.Slike).ToListAsync();

        public async Task<Proizvod?> GetByIdAsync(int id) =>
            await _context.Proizvodi.Include(p => p.Slike).FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Proizvod proizvod)
        {
            _context.Proizvodi.Add(proizvod);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Proizvod proizvod)
        {
            _context.Proizvodi.Update(proizvod);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var proizvod = await _context.Proizvodi.Include(p => p.Slike).FirstOrDefaultAsync(p => p.Id == id);
            if (proizvod != null)
            {
                _context.Proizvodi.Remove(proizvod);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProizvodSlika?> GetSlikaByIdAsync(int id) =>
            await _context.ProizvodSlike.FindAsync(id);

        public async Task AddSlikaAsync(ProizvodSlika slika)
        {
            _context.ProizvodSlike.Add(slika);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSlikaAsync(int id)
        {
            var slika = await _context.ProizvodSlike.FindAsync(id);
            if (slika != null)
            {
                _context.ProizvodSlike.Remove(slika);
                await _context.SaveChangesAsync();
            }
        }
    }
}
