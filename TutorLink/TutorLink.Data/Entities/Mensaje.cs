namespace TutorLink.Data.Entities;

public class Mensaje
{
    public int Id { get; set; }
    public int RemitenteId { get; set; }
    public Student? Remitente { get; set; }
    public int DestinatarioId { get; set; }
    public Student? Destinatario { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}