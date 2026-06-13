using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using PvcStolarija.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PvcStolarija.MVC.Controllers
{
    public class KupacController : Controller
    {
        private readonly IKupacService _kupacService;
        private readonly IFileUploadService _fileUploadService;

        public KupacController(IKupacService kupacService, IFileUploadService fileUploadService)
        {
            _kupacService = kupacService;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var kupci = await _kupacService.GetAllAsync();
            return View(kupci);
        }

        public async Task<IActionResult> Details(int id)
        {
            var kupac = await _kupacService.GetByIdAsync(id);
            if (kupac == null) return NotFound();
            return View(kupac);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kupac kupac, IFormFile profilnaSlika)
        {
            try
            {
                ModelState.Clear();
                if (profilnaSlika != null && profilnaSlika.Length > 0)
                    kupac.ProfilnaSlika = await _fileUploadService.UploadImageAsync(profilnaSlika, "kupci");
                await _kupacService.AddAsync(kupac);
                TempData["SuccessMessage"] = $"Kupac {kupac.Ime} {kupac.Prezime} je uspešno dodat!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(kupac); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var kupac = await _kupacService.GetByIdAsync(id);
            if (kupac == null) { TempData["ErrorMessage"] = "Kupac nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(kupac);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Kupac kupac, IFormFile profilnaSlika)
        {
            if (id != kupac.Id) { TempData["ErrorMessage"] = "Neispravni podaci."; return RedirectToAction(nameof(Index)); }
            try
            {
                if (profilnaSlika != null && profilnaSlika.Length > 0)
                {
                    if (!string.IsNullOrEmpty(kupac.ProfilnaSlika))
                        await _fileUploadService.DeleteImageAsync(kupac.ProfilnaSlika);
                    kupac.ProfilnaSlika = await _fileUploadService.UploadImageAsync(profilnaSlika, "kupci");
                }
                await _kupacService.UpdateAsync(kupac);
                TempData["SuccessMessage"] = $"Kupac {kupac.Ime} {kupac.Prezime} je uspešno izmenjen!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(kupac); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var kupac = await _kupacService.GetByIdAsync(id);
            if (kupac == null) return NotFound();
            return View(kupac);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var kupac = await _kupacService.GetByIdAsync(id);
                if (kupac?.ProfilnaSlika != null)
                    await _fileUploadService.DeleteImageAsync(kupac.ProfilnaSlika);
                await _kupacService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Kupac je uspešno obrisan!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            return RedirectToAction(nameof(Index));
        }
    }
}
