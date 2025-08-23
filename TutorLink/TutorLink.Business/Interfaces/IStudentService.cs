using TutorLink.Data.Entities;

namespace TutorLink.Business.Interfaces;

public interface IStudentService
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetAsync(int id);
    Task<Student> CreateAsync(Student s);
    Task<bool> UpdateAsync(Student s);
    Task<bool> DeleteAsync(int id);
}