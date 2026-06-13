using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using PvcStolarija.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PvcStolarija.MVC.Controllers
{
    public class RadnikController : Controller
    {
        private readonly IRadnikService _radnikService;
        private readonly IFileUploadService _fileUploadService;

        public RadnikController(IRadnikService radnikService, IFileUploadService fileUploadService)
        {
            _radnikService = radnikService;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var radnici = await _radnikService.GetAllAsync();
            return View(radnici);
        }

        public async Task<IActionResult> Details(int id)
        {
            var radnik = await _radnikService.GetByIdAsync(id);
            if (radnik == null) return NotFound();
            return View(radnik);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Radnik radnik, IFormFile profilnaSlika)
        {
            try
            {
                ModelState.Clear();
                if (profilnaSlika != null && profilnaSlika.Length > 0)
                    radnik.ProfilnaSlika = await _fileUploadService.UploadImageAsync(profilnaSlika, "radnici");
                await _radnikService.AddAsync(radnik);
                TempData["SuccessMessage"] = $"Radnik {radnik.Ime} {radnik.Prezime} je uspešno dodat!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška: {ex.Message}";
                return View(radnik);
            }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var radnik = await _radnikService.GetByIdAsync(id);
            if (radnik == null) { TempData["ErrorMessage"] = "Radnik nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(radnik);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Radnik radnik, IFormFile profilnaSlika)
        {
            if (id != radnik.Id) { TempData["ErrorMessage"] = "Neispravni podaci."; return RedirectToAction(nameof(Index)); }
            try
            {
                if (profilnaSlika != null && profilnaSlika.Length > 0)
                {
                    if (!string.IsNullOrEmpty(radnik.ProfilnaSlika))
                        await _fileUploadService.DeleteImageAsync(radnik.ProfilnaSlika);
                    radnik.ProfilnaSlika = await _fileUploadService.UploadImageAsync(profilnaSlika, "radnici");
                }
                await _radnikService.UpdateAsync(radnik);
                TempData["SuccessMessage"] = $"Radnik {radnik.Ime} {radnik.Prezime} je uspešno izmenjen!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(radnik); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var radnik = await _radnikService.GetByIdAsync(id);
            if (radnik == null) return NotFound();
            return View(radnik);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var radnik = await _radnikService.GetByIdAsync(id);
                if (radnik?.ProfilnaSlika != null)
                    await _fileUploadService.DeleteImageAsync(radnik.ProfilnaSlika);
                await _radnikService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Radnik je uspešno obrisan!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            return RedirectToAction(nameof(Index));
        }
    }
}
