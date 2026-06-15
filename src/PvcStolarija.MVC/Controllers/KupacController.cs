using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using PvcStolarija.MVC.Services;
using PvcStolarija.MVC.ViewModels.Kupac;

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

        private static KupacViewModel ToViewModel(Kupac k) => new()
        {
            Id = k.Id, Ime = k.Ime, Prezime = k.Prezime, JMBG = k.JMBG,
            Telefon = k.Telefon, Email = k.Email,
            DatumRegistracije = k.DatumRegistracije,
            ProfilnaSlika = k.ProfilnaSlika
        };

        private static Kupac ToEntity(KupacViewModel vm) => new()
        {
            Id = vm.Id, Ime = vm.Ime, Prezime = vm.Prezime, JMBG = vm.JMBG,
            Telefon = vm.Telefon, Email = vm.Email,
            DatumRegistracije = vm.DatumRegistracije,
            ProfilnaSlika = vm.ProfilnaSlika
        };

        public async Task<IActionResult> Index()
        {
            var kupci = await _kupacService.GetAllAsync();
            return View(kupci.Select(ToViewModel).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var kupac = await _kupacService.GetByIdAsync(id);
            if (kupac == null) return NotFound();
            return View(ToViewModel(kupac));
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create() => View(new KupacViewModel());

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KupacViewModel vm)
        {
            ModelState.Remove(nameof(vm.ProfilnaSlikaFajl));
            if (!ModelState.IsValid) return View(vm);
            try
            {
                if (vm.ProfilnaSlikaFajl != null && vm.ProfilnaSlikaFajl.Length > 0)
                    vm.ProfilnaSlika = await _fileUploadService.UploadImageAsync(vm.ProfilnaSlikaFajl, "kupci");
                await _kupacService.AddAsync(ToEntity(vm));
                TempData["SuccessMessage"] = $"Kupac {vm.Ime} {vm.Prezime} je uspešno dodat!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var kupac = await _kupacService.GetByIdAsync(id);
            if (kupac == null) { TempData["ErrorMessage"] = "Kupac nije pronađen."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(kupac));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KupacViewModel vm)
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
                    vm.ProfilnaSlika = await _fileUploadService.UploadImageAsync(vm.ProfilnaSlikaFajl, "kupci");
                }
                await _kupacService.UpdateAsync(ToEntity(vm));
                TempData["SuccessMessage"] = $"Kupac {vm.Ime} {vm.Prezime} je uspešno izmenjen!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var kupac = await _kupacService.GetByIdAsync(id);
            if (kupac == null) return NotFound();
            return View(ToViewModel(kupac));
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
