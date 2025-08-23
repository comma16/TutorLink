using Microsoft.AspNetCore.Mvc;
using TutorLink.Business.Interfaces;
using TutorLink.Data.Entities;

namespace TutorLink.Web.Controllers;

public class RatingsController : Controller
{
    private readonly IRatingService _svc;
    public RatingsController(IRatingService svc)=>_svc=svc;

    [HttpPost]
    public async Task<IActionResult> Create(Calificacion c, int redirectTutoriaId)
    {
        if (c.Puntaje < 1 || c.Puntaje > 5) ModelState.AddModelError("Puntaje", "Puntaje 1-5");
        if(!ModelState.IsValid) return RedirectToAction("Details","Tutorias", new { id = redirectTutoriaId });
        c.Fecha = DateTime.UtcNow;
        await _svc.AddAsync(c);
        return RedirectToAction("Details","Tutorias", new { id = redirectTutoriaId });
    }
}