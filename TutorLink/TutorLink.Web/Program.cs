using Microsoft.EntityFrameworkCore;
using TutorLink.Data;
using TutorLink.Business.Interfaces;
using TutorLink.Business.Services;
using TutorLink.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<TutorLinkContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITutoriaService, TutoriaService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IMessageService, MessageService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TutorLinkContext>();
    // Crea BD y tablas si no existen (sin migraciones).
    db.Database.EnsureCreated();

    // Seed simple si está vacío
    if (!db.Students.Any())
    {
        var juan = new Student { FirstName = "Juan", LastName = "Pérez", Email = "juan@demo.com", Career="Ingeniería", Skills="Matemáticas, Física", NeedsHelpIn="Programación" };
        var maria = new Student { FirstName = "María", LastName = "López", Email = "maria@demo.com", Career="Medicina", Skills="Biología", NeedsHelpIn="Estadística" };
        var carlos = new Student { FirstName = "Carlos", LastName = "Ruiz", Email = "carlos@demo.com", Career="Derecho", Skills="Derecho Civil", NeedsHelpIn="Inglés" };
        db.Students.AddRange(juan, maria, carlos);
        db.SaveChanges();

        var t1 = new Tutoria { Titulo = "Clases de Matemáticas nivel bachillerato", Materia="Matemáticas", Descripcion="Álgebra, trigonometría y cálculo básico", PrecioHora=400, StudentId=juan.Id };
        var t2 = new Tutoria { Titulo = "Biología para premed", Materia="Biología", Descripcion="Fisiología, anatomía, bioquímica básica", PrecioHora=500, StudentId=maria.Id };
        db.Tutorias.AddRange(t1, t2);
        db.SaveChanges();

        db.Calificaciones.AddRange(
            new Calificacion{ TutoriaId=t1.Id, StudentId=maria.Id, Puntaje=5, Comentario="Excelente explicación" },
            new Calificacion{ TutoriaId=t1.Id, StudentId=carlos.Id, Puntaje=4, Comentario="Muy claro" },
            new Calificacion{ TutoriaId=t2.Id, StudentId=juan.Id, Puntaje=5, Comentario="Dominio total del tema" }
        );
        db.Mensajes.Add(new Mensaje{ RemitenteId=juan.Id, DestinatarioId=maria.Id, Texto="Hola, ¿disponible el sábado?" });
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/health", () => Results.Ok("OK"));
app.Run();