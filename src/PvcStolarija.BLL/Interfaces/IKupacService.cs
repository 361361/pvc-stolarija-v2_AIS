using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Interfaces
{
    public interface IKupacService
    {
        Task<IEnumerable<Kupac>> GetAllAsync();
        Task<Kupac?> GetByIdAsync(int id);
        Task AddAsync(Kupac kupac);
        Task UpdateAsync(Kupac kupac);
        Task DeleteAsync(int id);
    }
}
