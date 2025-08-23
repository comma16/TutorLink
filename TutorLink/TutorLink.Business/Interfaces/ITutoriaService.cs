using TutorLink.Data.Entities;

namespace TutorLink.Business.Interfaces;

public interface ITutoriaService
{
    Task<List<Tutoria>> GetAllAsync();
    Task<Tutoria?> GetAsync(int id);
    Task<Tutoria> CreateAsync(Tutoria t);
    Task<bool> DeleteAsync(int id);
}