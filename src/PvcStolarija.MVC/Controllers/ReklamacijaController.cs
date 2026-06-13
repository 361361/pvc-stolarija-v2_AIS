using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PvcStolarija.MVC.Controllers
{
    [Authorize]
    public class ReklamacijaController : Controller
    {
        private readonly IReklamacijaService _reklamacijaService;
        public ReklamacijaController(IReklamacijaService reklamacijaService) => _reklamacijaService = reklamacijaService;

        public async Task<IActionResult> Index()
        {
            var reklamacije = await _reklamacijaService.GetAllAsync();
            return View(reklamacije);
        }

        public async Task<IActionResult> Details(int id)
        {
            var reklamacija = await _reklamacijaService.GetByIdAsync(id);
            if (reklamacija == null) { TempData["ErrorMessage"] = "Reklamacija nije pronađena."; return RedirectToAction(nameof(Index)); }
            return View(reklamacija);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            ViewBag.RadniciLista = await _reklamacijaService.GetAllRadniciAsync();
            ViewBag.ProizvodiLista = await _reklamacijaService.GetAllProizvodiAsync();
            ViewBag.Kupci = await _reklamacijaService.GetAllKupciAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reklamacija reklamacija, int kupacId, List<int>? selectedProizvodi)
        {
            ModelState.Remove("Radnik"); ModelState.Remove("KupacReklamacije"); ModelState.Remove("ReklamacijaProizvodi");
            if (ModelState.IsValid)
            {
                try
                {
                    await _reklamacijaService.AddAsync(reklamacija, kupacId, selectedProizvodi);
                    TempData["SuccessMessage"] = "Reklamacija uspešno dodata!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            }
            ViewBag.RadniciLista = await _reklamacijaService.GetAllRadniciAsync();
            ViewBag.ProizvodiLista = await _reklamacijaService.GetAllProizvodiAsync();
            ViewBag.Kupci = await _reklamacijaService.GetAllKupciAsync();
            return View(reklamacija);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var reklamacija = await _reklamacijaService.GetByIdAsync(id);
            if (reklamacija == null) { TempData["ErrorMessage"] = "Reklamacija nije pronađena."; return RedirectToAction(nameof(Index)); }
            ViewBag.RadniciLista = await _reklamacijaService.GetAllRadniciAsync();
            ViewBag.ProizvodiLista = await _reklamacijaService.GetAllProizvodiAsync();
            ViewBag.Kupci = await _reklamacijaService.GetAllKupciAsync();
            ViewBag.SelectedKupacId = reklamacija.KupacReklamacije.FirstOrDefault()?.KupacId ?? 0;
            ViewBag.SelectedProizvodi = reklamacija.ReklamacijaProizvodi.Select(rp => rp.ProizvodId).ToList();
            return View(reklamacija);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reklamacija reklamacija, int kupacId, List<int>? selectedProizvodi)
        {
            if (id != reklamacija.Id) return NotFound();
            ModelState.Remove("Radnik"); ModelState.Remove("KupacReklamacije"); ModelState.Remove("ReklamacijaProizvodi");
            if (ModelState.IsValid)
            {
                try
                {
                    await _reklamacijaService.UpdateAsync(reklamacija, kupacId, selectedProizvodi);
                    TempData["SuccessMessage"] = "Reklamacija uspešno ažurirana!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            }
            ViewBag.RadniciLista = await _reklamacijaService.GetAllRadniciAsync();
            ViewBag.ProizvodiLista = await _reklamacijaService.GetAllProizvodiAsync();
            ViewBag.Kupci = await _reklamacijaService.GetAllKupciAsync();
            ViewBag.SelectedKupacId = kupacId;
            ViewBag.SelectedProizvodi = selectedProizvodi;
            return View(reklamacija);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var reklamacija = await _reklamacijaService.GetByIdAsync(id);
            if (reklamacija == null) { TempData["ErrorMessage"] = "Reklamacija nije pronađena."; return RedirectToAction(nameof(Index)); }
            return View(reklamacija);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try { await _reklamacijaService.DeleteAsync(id); TempData["SuccessMessage"] = "Reklamacija uspešno obrisana!"; }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            return RedirectToAction(nameof(Index));
        }
    }
}
