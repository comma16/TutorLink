using Microsoft.AspNetCore.Mvc;
using TutorLink.Business.Interfaces;
using TutorLink.Data.Entities;

namespace TutorLink.Web.Controllers;

public class MessagesController : Controller
{
    private readonly IMessageService _svc;
    public MessagesController(IMessageService svc)=>_svc=svc;

    public IActionResult Create(int toStudentId)
    {
        return View(new Mensaje{ DestinatarioId = toStudentId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Mensaje m)
    {
        if(string.IsNullOrWhiteSpace(m.Texto)) ModelState.AddModelError("Texto","Escribe un mensaje");
        if(!ModelState.IsValid) return View(m);
        if (m.RemitenteId == 0) m.RemitenteId = 1; // demo
        await _svc.SendAsync(m);
        TempData["Ok"] = "Mensaje enviado";
        return RedirectToAction("Index","Tutorias");
    }
}