using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using PvcStolarija.MVC.ViewModels.Proizvod;

namespace PvcStolarija.MVC.Controllers
{
    public class ProizvodController : Controller
    {
        private readonly IProizvodService _proizvodService;
        private readonly IWebHostEnvironment _env;

        public ProizvodController(IProizvodService proizvodService, IWebHostEnvironment env)
        {
            _proizvodService = proizvodService;
            _env = env;
        }

        private static ProizvodViewModel ToViewModel(Proizvod p) => new()
        {
            Id = p.Id, Naziv = p.Naziv, Model = p.Model, Sifra = p.Sifra,
            GodinaProizvodnje = p.GodinaProizvodnje,
            TipMaterijala = p.TipMaterijala, Dostupnost = p.Dostupnost,
            Slike = p.Slike?.Select(s => new ProizvodSlikaViewModel
            {
                Id = s.Id, PutanjaDoSlike = s.PutanjaDoSlike, Opis = s.Opis
            }).ToList() ?? new()
        };

        private static Proizvod ToEntity(ProizvodViewModel vm) => new()
        {
            Id = vm.Id, Naziv = vm.Naziv, Model = vm.Model, Sifra = vm.Sifra,
            GodinaProizvodnje = vm.GodinaProizvodnje,
            TipMaterijala = vm.TipMaterijala, Dostupnost = vm.Dostupnost
        };

        public async Task<IActionResult> Index()
        {
            var proizvodi = await _proizvodService.GetAllAsync();
            return View(proizvodi.Select(ToViewModel).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var p = await _proizvodService.GetByIdAsync(id);
            if (p == null) { TempData["ErrorMessage"] = "Proizvod nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(p));
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() => View(new ProizvodViewModel());

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProizvodViewModel vm)
        {
            ModelState.Remove(nameof(vm.NoveSlike));
            ModelState.Remove(nameof(vm.Slike));
            if (!ModelState.IsValid) return View(vm);
            try
            {
                var entity = ToEntity(vm);
                await _proizvodService.AddAsync(entity);

                if (vm.NoveSlike?.Any() == true)
                    await SaveSlike(entity.Id, vm.NoveSlike, vm.OpisiNovihSlika);

                TempData["SuccessMessage"] = "Proizvod je uspešno dodat!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _proizvodService.GetByIdAsync(id);
            if (p == null) { TempData["ErrorMessage"] = "Proizvod nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(p));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProizvodViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            ModelState.Remove(nameof(vm.NoveSlike));
            ModelState.Remove(nameof(vm.Slike));
            if (!ModelState.IsValid)
            {
                var existing = await _proizvodService.GetByIdAsync(id);
                if (existing != null) vm.Slike = ToViewModel(existing).Slike;
                return View(vm);
            }
            try
            {
                await _proizvodService.UpdateAsync(ToEntity(vm));
                if (vm.NoveSlike?.Any() == true)
                    await SaveSlike(vm.Id, vm.NoveSlike, vm.OpisiNovihSlika);

                TempData["SuccessMessage"] = "Proizvod je uspešno ažuriran!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _proizvodService.GetByIdAsync(id);
            if (p == null) { TempData["ErrorMessage"] = "Proizvod nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(p));
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var p = await _proizvodService.GetByIdAsync(id);
                if (p?.Slike?.Any() == true)
                    foreach (var s in p.Slike) DeleteFile(s.PutanjaDoSlike);
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
                DeleteFile(slika.PutanjaDoSlike);
                await _proizvodService.DeleteSlikaAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        private async Task SaveSlike(int proizvodId, List<IFormFile> slike, List<string>? opisi)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var folder = Path.Combine(webRoot, "uploads", "proizvodi");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            for (int i = 0; i < slike.Count; i++)
            {
                var file = slike[i];
                if (file.Length == 0) continue;
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await file.CopyToAsync(stream);
                await _proizvodService.AddSlikaAsync(new ProizvodSlika
                {
                    ProizvodId = proizvodId,
                    PutanjaDoSlike = "/uploads/proizvodi/" + fileName,
                    Opis = opisi != null && i < opisi.Count ? opisi[i] : null
                });
            }
        }

        private void DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var full = Path.Combine(webRoot, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(full)) System.IO.File.Delete(full);
        }
    }
}
