using Microsoft.EntityFrameworkCore;
using TutorLink.Business.Interfaces;
using TutorLink.Data;
using TutorLink.Data.Entities;

namespace TutorLink.Business.Services;

public class StudentService : IStudentService
{
    private readonly TutorLinkContext _db;
    public StudentService(TutorLinkContext db) => _db = db;

    public async Task<List<Student>> GetAllAsync() => await _db.Students.AsNoTracking().OrderBy(s => s.FirstName).ToListAsync();
    public async Task<Student?> GetAsync(int id) => await _db.Students.FindAsync(id);
    public async Task<Student> CreateAsync(Student s) { _db.Students.Add(s); await _db.SaveChangesAsync(); return s; }
    public async Task<bool> UpdateAsync(Student s) { if(!await _db.Students.AnyAsync(x=>x.Id==s.Id)) return false; _db.Entry(s).State=EntityState.Modified; await _db.SaveChangesAsync(); return true; }
    public async Task<bool> DeleteAsync(int id) { var e = await _db.Students.FindAsync(id); if(e==null) return false; _db.Students.Remove(e); await _db.SaveChangesAsync(); return true; }
}

public class TutoriaService : ITutoriaService
{
    private readonly TutorLinkContext _db;
    public TutoriaService(TutorLinkContext db)=>_db=db;

    public async Task<List<Tutoria>> GetAllAsync() => await _db.Tutorias.Include(t=>t.Student).Include(t=>t.Calificaciones).AsNoTracking().ToListAsync();
    public async Task<Tutoria?> GetAsync(int id) => await _db.Tutorias.Include(t=>t.Student).Include(t=>t.Calificaciones).ThenInclude(c=>c.Student).FirstOrDefaultAsync(t=>t.Id==id);
    public async Task<Tutoria> CreateAsync(Tutoria t) { _db.Tutorias.Add(t); await _db.SaveChangesAsync(); return t; }
    public async Task<bool> DeleteAsync(int id) { var e = await _db.Tutorias.FindAsync(id); if(e==null) return false; _db.Tutorias.Remove(e); await _db.SaveChangesAsync(); return true; }
}

public class RatingService : IRatingService
{
    private readonly TutorLinkContext _db;
    public RatingService(TutorLinkContext db)=>_db=db;

    public async Task<Calificacion> AddAsync(Calificacion c) { _db.Calificaciones.Add(c); await _db.SaveChangesAsync(); return c; }
    public async Task<double> AverageForTutoriaAsync(int tutoriaId)
        => await _db.Calificaciones.Where(c=>c.TutoriaId==tutoriaId).Select(c => (double?)c.Puntaje).AverageAsync() ?? 0.0;
}

public class MessageService : IMessageService
{
    private readonly TutorLinkContext _db;
    public MessageService(TutorLinkContext db)=>_db=db;

    public async Task<Mensaje> SendAsync(Mensaje m) { _db.Mensajes.Add(m); await _db.SaveChangesAsync(); return m; }
    public async Task<List<Mensaje>> InboxAsync(int studentId)
        => await _db.Mensajes.Where(x=>x.DestinatarioId==studentId).Include(x=>x.Remitente).OrderByDescending(x=>x.Fecha).ToListAsync();
}