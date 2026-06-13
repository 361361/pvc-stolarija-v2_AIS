using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PvcStolarija.MVC.Controllers
{
    public class ProizvodController : Controller
    {
        private readonly IProizvodService _proizvodService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProizvodController(IProizvodService proizvodService, IWebHostEnvironment webHostEnvironment)
        {
            _proizvodService = proizvodService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var proizvodi = await _proizvodService.GetAllAsync();
            return View(proizvodi);
        }

        public async Task<IActionResult> Details(int id)
        {
            var proizvod = await _proizvodService.GetByIdAsync(id);
            if (proizvod == null) { TempData["ErrorMessage"] = "Proizvod nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(proizvod);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Proizvod proizvod, List<IFormFile>? slike, List<string>? opisiSlika)
        {
            ModelState.Remove("Slike");
            ModelState.Remove("Narudzbine");
            ModelState.Remove("ReklamacijaProizvodi");
            if (ModelState.IsValid)
            {
                try
                {
                    await _proizvodService.AddAsync(proizvod);
                    if (slike?.Any() == true)
                        await SaveProizvodSlike(proizvod.Id, slike, opisiSlika);
                    TempData["SuccessMessage"] = "Proizvod je uspešno dodat!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            }
            return View(proizvod);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var proizvod = await _proizvodService.GetByIdAsync(id);
            if (proizvod == null) { TempData["ErrorMessage"] = "Proizvod nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(proizvod);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Proizvod proizvod, List<IFormFile>? noveSlike, List<string>? opisiNovihSlika)
        {
            if (id != proizvod.Id) return NotFound();
            ModelState.Remove("Slike"); ModelState.Remove("Narudzbine"); ModelState.Remove("ReklamacijaProizvodi");
            if (ModelState.IsValid)
            {
                try
                {
                    await _proizvodService.UpdateAsync(proizvod);
                    if (noveSlike?.Any() == true)
                        await SaveProizvodSlike(proizvod.Id, noveSlike, opisiNovihSlika);
                    TempData["SuccessMessage"] = "Proizvod je uspešno ažuriran!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            }
            var pSaSlikama = await _proizvodService.GetByIdAsync(id);
            proizvod.Slike = pSaSlikama?.Slike;
            return View(proizvod);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var proizvod = await _proizvodService.GetByIdAsync(id);
            if (proizvod == null) { TempData["ErrorMessage"] = "Proizvod nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(proizvod);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var proizvod = await _proizvodService.GetByIdAsync(id);
                if (proizvod?.Slike?.Any() == true)
                    foreach (var slika in proizvod.Slike)
                        DeleteImageFile(slika.PutanjaDoSlike);
                await _proizvodService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Proizvod je uspešno obrisan!";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var slika = await _proizvodService.GetSlikaByIdAsync(id);
                if (slika == null) return Json(new { success = false, message = "Slika nije pronađena." });
                DeleteImageFile(slika.PutanjaDoSlike);
                await _proizvodService.DeleteSlikaAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        private async Task SaveProizvodSlike(int proizvodId, List<IFormFile> slike, List<string>? opisi)
        {
            var webRoot = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            string uploadsFolder = Path.Combine(webRoot, "uploads", "proizvodi");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            for (int i = 0; i < slike.Count; i++)
            {
                var file = slike[i];
                if (file.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fs = new FileStream(filePath, FileMode.Create)) await file.CopyToAsync(fs);
                    await _proizvodService.AddSlikaAsync(new ProizvodSlika
                    {
                        ProizvodId = proizvodId,
                        PutanjaDoSlike = "/uploads/proizvodi/" + uniqueFileName,
                        Opis = opisi != null && i < opisi.Count ? opisi[i] : null
                    });
                }
            }
        }

        private void DeleteImageFile(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var webRoot2 = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
                string fullPath = Path.Combine(webRoot2, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }
        }
    }
}
