namespace TutorLink.Data.Entities;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Career { get; set; }
    public string? Skills { get; set; } // materias que domina
    public string? NeedsHelpIn { get; set; } // materias donde necesita ayuda
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Tutoria> Tutorias { get; set; } = new List<Tutoria>();
    public ICollection<Mensaje> MensajesEnviados { get; set; } = new List<Mensaje>();
    public ICollection<Mensaje> MensajesRecibidos { get; set; } = new List<Mensaje>();
    public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
}