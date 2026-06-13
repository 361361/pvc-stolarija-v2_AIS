using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Interfaces
{
    public interface INarudzbaService
    {
        Task<IEnumerable<Narudzba>> GetAllAsync();
        Task<Narudzba?> GetByIdAsync(int id);
        Task AddAsync(Narudzba narudzba, List<int> kupacIds);
        Task UpdateAsync(Narudzba narudzba, List<(int KupacId, bool Potvrdjen, string? Napomena)> kupacNarudzbine);
        Task DeleteAsync(int id);
        Task<IEnumerable<Radnik>> GetAllRadniciAsync();
        Task<IEnumerable<Kupac>> GetAllKupciAsync();
        Task<IEnumerable<Proizvod>> GetAllProizvodiAsync();
    }
}
