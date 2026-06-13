using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Interfaces
{
    public interface IProizvodService
    {
        Task<IEnumerable<Proizvod>> GetAllAsync();
        Task<Proizvod?> GetByIdAsync(int id);
        Task AddAsync(Proizvod proizvod);
        Task UpdateAsync(Proizvod proizvod);
        Task DeleteAsync(int id);
        Task<ProizvodSlika?> GetSlikaByIdAsync(int id);
        Task AddSlikaAsync(ProizvodSlika slika);
        Task DeleteSlikaAsync(int id);
    }
}
