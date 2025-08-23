using TutorLink.Data.Entities;

namespace TutorLink.Business.Interfaces;

public interface IRatingService
{
    Task<Calificacion> AddAsync(Calificacion c);
    Task<double> AverageForTutoriaAsync(int tutoriaId);
}