using Microsoft.EntityFrameworkCore;
using TutorLink.Data.Entities;

namespace TutorLink.Data;

public class TutorLinkContext : DbContext
{
    public TutorLinkContext(DbContextOptions<TutorLinkContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Tutoria> Tutorias => Set<Tutoria>();
    public DbSet<Calificacion> Calificaciones => Set<Calificacion>();
    public DbSet<Mensaje> Mensajes => Set<Mensaje>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tutoria>()
            .HasOne(t => t.Student)
            .WithMany(s => s.Tutorias)
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Calificacion>()
            .HasOne(c => c.Tutoria)
            .WithMany(t => t.Calificaciones)
            .HasForeignKey(c => c.TutoriaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Calificacion>()
            .HasOne(c => c.Student)
            .WithMany(s => s.Calificaciones)
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Mensaje>()
            .HasOne(m => m.Remitente)
            .WithMany(s => s.MensajesEnviados)
            .HasForeignKey(m => m.RemitenteId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Mensaje>()
            .HasOne(m => m.Destinatario)
            .WithMany(s => s.MensajesRecibidos)
            .HasForeignKey(m => m.DestinatarioId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}