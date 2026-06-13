using Microsoft.EntityFrameworkCore;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Data;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Services
{
    public class ReklamacijaService : IReklamacijaService
    {
        private readonly PvcStolarijaDbContext _context;
        public ReklamacijaService(PvcStolarijaDbContext context) => _context = context;

        public async Task<IEnumerable<Reklamacija>> GetAllAsync() =>
            await _context.Reklamacije
                .Include(r => r.Radnik)
                .Include(r => r.KupacReklamacije).ThenInclude(kr => kr.Kupac)
                .Include(r => r.ReklamacijaProizvodi).ThenInclude(rp => rp.Proizvod).ThenInclude(p => p.Slike)
                .ToListAsync();

        public async Task<Reklamacija?> GetByIdAsync(int id) =>
            await _context.Reklamacije
                .Include(r => r.Radnik)
                .Include(r => r.KupacReklamacije).ThenInclude(kr => kr.Kupac)
                .Include(r => r.ReklamacijaProizvodi).ThenInclude(rp => rp.Proizvod).ThenInclude(p => p.Slike)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task AddAsync(Reklamacija reklamacija, int kupacId, List<int>? proizvodiIds)
        {
            if (reklamacija.TipReklamacije == TipReklamacije.Terenska && (proizvodiIds == null || !proizvodiIds.Any()))
                throw new InvalidOperationException("Terenska reklamacija mora imati bar jedan proizvod!");
            if (reklamacija.TipReklamacije == TipReklamacije.Pisana && proizvodiIds?.Any() == true)
                throw new InvalidOperationException("Pisana reklamacija ne može imati proizvode!");

            _context.Reklamacije.Add(reklamacija);
            await _context.SaveChangesAsync();

            _context.KupacReklamacije.Add(new KupacReklamacija { ReklamacijaId = reklamacija.Id, KupacId = kupacId });
            if (proizvodiIds?.Any() == true)
                foreach (var pid in proizvodiIds)
                    _context.ReklamacijaProizvodi.Add(new ReklamacijaProizvod { ReklamacijaId = reklamacija.Id, ProizvodId = pid });

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reklamacija reklamacija, int kupacId, List<int>? proizvodiIds)
        {
            if (reklamacija.TipReklamacije == TipReklamacije.Terenska && (proizvodiIds == null || !proizvodiIds.Any()))
                throw new InvalidOperationException("Terenska reklamacija mora imati bar jedan proizvod!");
            if (reklamacija.TipReklamacije == TipReklamacije.Pisana && proizvodiIds?.Any() == true)
                throw new InvalidOperationException("Pisana reklamacija ne može imati proizvode!");

            _context.Reklamacije.Update(reklamacija);
            _context.KupacReklamacije.RemoveRange(await _context.KupacReklamacije.Where(kr => kr.ReklamacijaId == reklamacija.Id).ToListAsync());
            _context.ReklamacijaProizvodi.RemoveRange(await _context.ReklamacijaProizvodi.Where(rp => rp.ReklamacijaId == reklamacija.Id).ToListAsync());

            _context.KupacReklamacije.Add(new KupacReklamacija { ReklamacijaId = reklamacija.Id, KupacId = kupacId });
            if (proizvodiIds?.Any() == true)
                foreach (var pid in proizvodiIds)
                    _context.ReklamacijaProizvodi.Add(new ReklamacijaProizvod { ReklamacijaId = reklamacija.Id, ProizvodId = pid });

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var r = await _context.Reklamacije.FindAsync(id);
            if (r != null) { _context.Reklamacije.Remove(r); await _context.SaveChangesAsync(); }
        }

        public async Task<IEnumerable<Radnik>> GetAllRadniciAsync() => await _context.Radnici.ToListAsync();
        public async Task<IEnumerable<Kupac>> GetAllKupciAsync() => await _context.Kupci.ToListAsync();
        public async Task<IEnumerable<Proizvod>> GetAllProizvodiAsync() =>
            await _context.Proizvodi.Where(p => p.Dostupnost == Dostupnost.Dostupan).Include(p => p.Slike).ToListAsync();
    }
}
