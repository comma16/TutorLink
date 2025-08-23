namespace TutorLink.Data.Entities;

public class Tutoria
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Materia { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal PrecioHora { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
}