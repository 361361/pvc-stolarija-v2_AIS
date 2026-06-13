using Microsoft.EntityFrameworkCore;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Data;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Services
{
    public class NarudzbaService : INarudzbaService
    {
        private readonly PvcStolarijaDbContext _context;
        public NarudzbaService(PvcStolarijaDbContext context) => _context = context;

        public async Task<IEnumerable<Narudzba>> GetAllAsync() =>
            await _context.Narudzbine
                .Include(n => n.Radnik)
                .Include(n => n.Proizvod).ThenInclude(p => p.Slike)
                .Include(n => n.KupacNarudzbine).ThenInclude(kn => kn.Kupac)
                .ToListAsync();

        public async Task<Narudzba?> GetByIdAsync(int id) =>
            await _context.Narudzbine
                .Include(n => n.Radnik)
                .Include(n => n.Proizvod).ThenInclude(p => p.Slike)
                .Include(n => n.KupacNarudzbine).ThenInclude(kn => kn.Kupac)
                .FirstOrDefaultAsync(n => n.Id == id);

        public async Task AddAsync(Narudzba narudzba, List<int> kupacIds)
        {
            _context.Narudzbine.Add(narudzba);
            await _context.SaveChangesAsync();

            if (kupacIds?.Any() == true)
            {
                foreach (var kupacId in kupacIds)
                    _context.KupacNarudzbine.Add(new KupacNarudzba { NarudzbaId = narudzba.Id, KupacId = kupacId, Potvrdjen = false });
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Narudzba narudzba, List<(int KupacId, bool Potvrdjen, string? Napomena)> kupacNarudzbine)
        {
            _context.Narudzbine.Update(narudzba);
            var stari = await _context.KupacNarudzbine.Where(kn => kn.NarudzbaId == narudzba.Id).ToListAsync();
            _context.KupacNarudzbine.RemoveRange(stari);

            if (kupacNarudzbine?.Any() == true)
                foreach (var (kupacId, potvrdjen, napomena) in kupacNarudzbine)
                    _context.KupacNarudzbine.Add(new KupacNarudzba { NarudzbaId = narudzba.Id, KupacId = kupacId, Potvrdjen = potvrdjen, Napomena = napomena });

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var narudzba = await _context.Narudzbine.FindAsync(id);
            if (narudzba != null) { _context.Narudzbine.Remove(narudzba); await _context.SaveChangesAsync(); }
        }

        public async Task<IEnumerable<Radnik>> GetAllRadniciAsync() => await _context.Radnici.ToListAsync();
        public async Task<IEnumerable<Kupac>> GetAllKupciAsync() => await _context.Kupci.ToListAsync();
        public async Task<IEnumerable<Proizvod>> GetAllProizvodiAsync() =>
            await _context.Proizvodi.Where(p => p.Dostupnost == Dostupnost.Dostupan).ToListAsync();
    }
}
