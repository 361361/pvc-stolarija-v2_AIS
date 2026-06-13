using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PvcStolarija.MVC.Controllers
{
    [Authorize]
    public class NarudzbaController : Controller
    {
        private readonly INarudzbaService _narudzbaService;
        public NarudzbaController(INarudzbaService narudzbaService) => _narudzbaService = narudzbaService;

        public async Task<IActionResult> Index()
        {
            var narudzbine = await _narudzbaService.GetAllAsync();
            return View(narudzbine);
        }

        public async Task<IActionResult> Details(int id)
        {
            var narudzba = await _narudzbaService.GetByIdAsync(id);
            if (narudzba == null) { TempData["ErrorMessage"] = "Narudžba nije pronađena."; return RedirectToAction(nameof(Index)); }
            return View(narudzba);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            ViewBag.RadniciLista = (await _narudzbaService.GetAllRadniciAsync()).ToList();
            ViewBag.ProizvodiLista = (await _narudzbaService.GetAllProizvodiAsync()).ToList();
            ViewBag.Kupci = await _narudzbaService.GetAllKupciAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Narudzba narudzba, List<int>? selectedKupci)
        {
            ModelState.Remove("Radnik"); ModelState.Remove("Proizvod"); ModelState.Remove("KupacNarudzbine");
            if (ModelState.IsValid)
            {
                try
                {
                    await _narudzbaService.AddAsync(narudzba, selectedKupci ?? new List<int>());
                    TempData["SuccessMessage"] = "Narudžba je uspešno dodata!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            }
            ViewBag.RadniciLista = (await _narudzbaService.GetAllRadniciAsync()).ToList();
            ViewBag.ProizvodiLista = (await _narudzbaService.GetAllProizvodiAsync()).ToList();
            ViewBag.Kupci = await _narudzbaService.GetAllKupciAsync();
            return View(narudzba);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var narudzba = await _narudzbaService.GetByIdAsync(id);
            if (narudzba == null) { TempData["ErrorMessage"] = "Narudžba nije pronađena."; return RedirectToAction(nameof(Index)); }
            ViewBag.RadniciLista = (await _narudzbaService.GetAllRadniciAsync()).ToList();
            ViewBag.ProizvodiLista = (await _narudzbaService.GetAllProizvodiAsync()).ToList();
            ViewBag.Kupci = await _narudzbaService.GetAllKupciAsync();
            ViewBag.SelectedKupci = narudzba.KupacNarudzbine?.Select(kn => kn.KupacId).ToList() ?? new List<int>();
            return View(narudzba);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Narudzba narudzba, List<int>? selectedKupci)
        {
            if (id != narudzba.Id) return NotFound();
            ModelState.Remove("Radnik"); ModelState.Remove("Proizvod"); ModelState.Remove("KupacNarudzbine");
            if (ModelState.IsValid)
            {
                try
                {
                    var kupacNarudzbineData = new List<(int KupacId, bool Potvrdjen, string? Napomena)>();
                    if (selectedKupci != null)
                        foreach (var kupacId in selectedKupci)
                        {
                            bool potvrdjen = Request.Form[$"potvrdjen_{kupacId}"].ToString() == "on";
                            string? napomena = Request.Form[$"napomene_{kupacId}"].ToString();
                            kupacNarudzbineData.Add((kupacId, potvrdjen, napomena));
                        }
                    await _narudzbaService.UpdateAsync(narudzba, kupacNarudzbineData);
                    TempData["SuccessMessage"] = "Narudžba je uspešno ažurirana!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            }
            ViewBag.RadniciLista = (await _narudzbaService.GetAllRadniciAsync()).ToList();
            ViewBag.ProizvodiLista = (await _narudzbaService.GetAllProizvodiAsync()).ToList();
            ViewBag.Kupci = await _narudzbaService.GetAllKupciAsync();
            ViewBag.SelectedKupci = selectedKupci ?? new List<int>();
            return View(narudzba);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var narudzba = await _narudzbaService.GetByIdAsync(id);
            if (narudzba == null) { TempData["ErrorMessage"] = "Narudžba nije pronađena."; return RedirectToAction(nameof(Index)); }
            return View(narudzba);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try { await _narudzbaService.DeleteAsync(id); TempData["SuccessMessage"] = "Narudžba je uspešno obrisana!"; }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            return RedirectToAction(nameof(Index));
        }
    }
}
