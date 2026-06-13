using PvcStolarija.DAL.Models;

namespace PvcStolarija.BLL.Interfaces
{
    public interface IRadnikService
    {
        Task<IEnumerable<Radnik>> GetAllAsync();
        Task<Radnik?> GetByIdAsync(int id);
        Task AddAsync(Radnik radnik);
        Task UpdateAsync(Radnik radnik);
        Task DeleteAsync(int id);
    }
}
