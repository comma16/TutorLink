using Microsoft.AspNetCore.Mvc;
using TutorLink.Business.Interfaces;
using TutorLink.Data.Entities;

namespace TutorLink.Web.Controllers;

public class TutoriasController : Controller
{
    private readonly ITutoriaService _svc;
    private readonly IRatingService _ratings;

    public TutoriasController(ITutoriaService svc, IRatingService ratings)
    {
        _svc = svc; _ratings = ratings;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _svc.GetAllAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var t = await _svc.GetAsync(id);
        if (t == null) return NotFound();
        ViewBag.Average = await _ratings.AverageForTutoriaAsync(id);
        return View(t);
    }

    public IActionResult Create() => View(new Tutoria());

    [HttpPost]
    public async Task<IActionResult> Create(Tutoria t)
    {
        if(!ModelState.IsValid) return View(t);
        await _svc.CreateAsync(t);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var t = await _svc.GetAsync(id);
        if (t == null) return NotFound();
        return View(t);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _svc.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}