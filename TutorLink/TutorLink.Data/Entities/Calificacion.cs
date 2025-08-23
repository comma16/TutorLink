namespace TutorLink.Data.Entities;

public class Calificacion
{
    public int Id { get; set; }
    public int Puntaje { get; set; } // 1-5
    public string? Comentario { get; set; }
    public int TutoriaId { get; set; }
    public Tutoria? Tutoria { get; set; }
    public int StudentId { get; set; } // quien califica
    public Student? Student { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}