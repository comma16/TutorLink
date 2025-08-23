using Microsoft.AspNetCore.Mvc;
using TutorLink.Business.Interfaces;
using TutorLink.Data.Entities;

namespace TutorLink.Web.Controllers;

public class StudentsController : Controller
{
    private readonly IStudentService _svc;
    public StudentsController(IStudentService svc) => _svc = svc;

    public async Task<IActionResult> Index() => View(await _svc.GetAllAsync());
    public IActionResult Create() => View(new Student());

    [HttpPost]
    public async Task<IActionResult> Create(Student s)
    {
        if(!ModelState.IsValid) return View(s);
        await _svc.CreateAsync(s);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var s = await _svc.GetAsync(id);
        if (s == null) return NotFound();
        return View(s);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Student s)
    {
        if(!ModelState.IsValid) return View(s);
        var ok = await _svc.UpdateAsync(s);
        if(!ok) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var s = await _svc.GetAsync(id);
        if (s == null) return NotFound();
        return View(s);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _svc.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}