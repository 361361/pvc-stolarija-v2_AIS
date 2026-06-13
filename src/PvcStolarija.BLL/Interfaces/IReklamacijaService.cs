using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Interfaces
{
    public interface IReklamacijaService
    {
        Task<IEnumerable<Reklamacija>> GetAllAsync();
        Task<Reklamacija?> GetByIdAsync(int id);
        Task AddAsync(Reklamacija reklamacija, int kupacId, List<int>? proizvodiIds);
        Task UpdateAsync(Reklamacija reklamacija, int kupacId, List<int>? proizvodiIds);
        Task DeleteAsync(int id);
        Task<IEnumerable<Radnik>> GetAllRadniciAsync();
        Task<IEnumerable<Kupac>> GetAllKupciAsync();
        Task<IEnumerable<Proizvod>> GetAllProizvodiAsync();
    }
}
