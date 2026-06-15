using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using PvcStolarija.MVC.Services;
using PvcStolarija.MVC.ViewModels.Radnik;

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

        // ── Mapiranje: Entity → ViewModel ──────────────────────
        private static RadnikViewModel ToViewModel(Radnik r) => new()
        {
            Id = r.Id, Ime = r.Ime, Prezime = r.Prezime, JMBG = r.JMBG,
            Telefon = r.Telefon, Email = r.Email,
            BrojUgovora = r.BrojUgovora, GodineRadnogIskustva = r.GodineRadnogIskustva,
            ProfilnaSlika = r.ProfilnaSlika
        };

        // ── Mapiranje: ViewModel → Entity ──────────────────────
        private static Radnik ToEntity(RadnikViewModel vm) => new()
        {
            Id = vm.Id, Ime = vm.Ime, Prezime = vm.Prezime, JMBG = vm.JMBG,
            Telefon = vm.Telefon, Email = vm.Email,
            BrojUgovora = vm.BrojUgovora, GodineRadnogIskustva = vm.GodineRadnogIskustva,
            ProfilnaSlika = vm.ProfilnaSlika
        };

        public async Task<IActionResult> Index()
        {
            var radnici = await _radnikService.GetAllAsync();
            var viewModels = radnici.Select(ToViewModel).ToList();
            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var radnik = await _radnikService.GetByIdAsync(id);
            if (radnik == null) return NotFound();
            return View(ToViewModel(radnik));
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() => View(new RadnikViewModel());

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RadnikViewModel vm)
        {
            ModelState.Remove(nameof(vm.ProfilnaSlikaFajl));
            if (!ModelState.IsValid) return View(vm);
            try
            {
                if (vm.ProfilnaSlikaFajl != null && vm.ProfilnaSlikaFajl.Length > 0)
                    vm.ProfilnaSlika = await _fileUploadService.UploadImageAsync(vm.ProfilnaSlikaFajl, "radnici");

                await _radnikService.AddAsync(ToEntity(vm));
                TempData["SuccessMessage"] = $"Radnik {vm.Ime} {vm.Prezime} je uspešno dodat!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var radnik = await _radnikService.GetByIdAsync(id);
            if (radnik == null) { TempData["ErrorMessage"] = "Radnik nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(radnik));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RadnikViewModel vm)
        {
            if (id != vm.Id) { TempData["ErrorMessage"] = "Neispravni podaci."; return RedirectToAction(nameof(Index)); }
            ModelState.Remove(nameof(vm.ProfilnaSlikaFajl));
            if (!ModelState.IsValid) return View(vm);
            try
            {
                if (vm.ProfilnaSlikaFajl != null && vm.ProfilnaSlikaFajl.Length > 0)
                {
                    if (!string.IsNullOrEmpty(vm.ProfilnaSlika))
                        await _fileUploadService.DeleteImageAsync(vm.ProfilnaSlika);
                    vm.ProfilnaSlika = await _fileUploadService.UploadImageAsync(vm.ProfilnaSlikaFajl, "radnici");
                }
                await _radnikService.UpdateAsync(ToEntity(vm));
                TempData["SuccessMessage"] = $"Radnik {vm.Ime} {vm.Prezime} je uspešno izmenjen!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var radnik = await _radnikService.GetByIdAsync(id);
            if (radnik == null) return NotFound();
            return View(ToViewModel(radnik));
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
