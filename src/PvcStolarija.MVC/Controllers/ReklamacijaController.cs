using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using PvcStolarija.MVC.ViewModels.Reklamacija;

namespace PvcStolarija.MVC.Controllers
{
    [Authorize]
    public class ReklamacijaController : Controller
    {
        private readonly IReklamacijaService _reklamacijaService;
        public ReklamacijaController(IReklamacijaService reklamacijaService) => _reklamacijaService = reklamacijaService;

        private static ReklamacijaViewModel ToViewModel(Reklamacija r) => new()
        {
            Id = r.Id, Datum = r.Datum, TipReklamacije = r.TipReklamacije,
            Prioritet = r.Prioritet, RadnikId = r.RadnikId,
            RadnikImePrezime = r.Radnik != null ? $"{r.Radnik.Ime} {r.Radnik.Prezime}" : null,
            KupacId = r.KupacReklamacije?.FirstOrDefault()?.KupacId ?? 0,
            KupacImePrezime = r.KupacReklamacije?.FirstOrDefault()?.Kupac is { } k ? $"{k.Ime} {k.Prezime}" : null,
            SelectedProizvodiIds = r.ReklamacijaProizvodi?.Select(rp => rp.ProizvodId).ToList() ?? new(),
            Proizvodi = r.ReklamacijaProizvodi?.Select(rp => new ReklamacijaProizvodViewModel
            {
                Id = rp.ProizvodId, Naziv = $"{rp.Proizvod.Naziv} {rp.Proizvod.Model}",
                Sifra = rp.Proizvod.Sifra,
                PrvaSlika = rp.Proizvod.Slike?.FirstOrDefault()?.PutanjaDoSlike
            }).ToList() ?? new()
        };

        private async Task PopulateListe(ReklamacijaViewModel vm)
        {
            vm.RadniciLista = (await _reklamacijaService.GetAllRadniciAsync())
                .Select(r => new ReklamacijaRadnikItem { Id = r.Id, ImePrezime = $"{r.Ime} {r.Prezime}" }).ToList();
            vm.KupciLista = (await _reklamacijaService.GetAllKupciAsync())
                .Select(k => new ReklamacijaKupacItem { Id = k.Id, ImePrezime = $"{k.Ime} {k.Prezime}", JMBG = k.JMBG }).ToList();
            vm.ProizvodiLista = (await _reklamacijaService.GetAllProizvodiAsync())
                .Select(p => new ReklamacijaProizvodItem { Id = p.Id, Naziv = $"{p.Naziv} {p.Model}", Sifra = p.Sifra }).ToList();
        }

        public async Task<IActionResult> Index()
        {
            var reklamacije = await _reklamacijaService.GetAllAsync();
            return View(reklamacije.Select(ToViewModel).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var r = await _reklamacijaService.GetByIdAsync(id);
            if (r == null) { TempData["ErrorMessage"] = "Reklamacija nije pronađena."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(r));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            var vm = new ReklamacijaViewModel();
            await PopulateListe(vm);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReklamacijaViewModel vm)
        {
            ModelState.Remove(nameof(vm.RadniciLista));
            ModelState.Remove(nameof(vm.KupciLista));
            ModelState.Remove(nameof(vm.ProizvodiLista));
            ModelState.Remove(nameof(vm.Proizvodi));
            if (!ModelState.IsValid) { await PopulateListe(vm); return View(vm); }
            try
            {
                var entity = new Reklamacija
                {
                    Datum = vm.Datum, TipReklamacije = vm.TipReklamacije,
                    Prioritet = vm.Prioritet, RadnikId = vm.RadnikId
                };
                await _reklamacijaService.AddAsync(entity, vm.KupacId, vm.SelectedProizvodiIds);
                TempData["SuccessMessage"] = "Reklamacija uspešno dodata!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; await PopulateListe(vm); return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var r = await _reklamacijaService.GetByIdAsync(id);
            if (r == null) { TempData["ErrorMessage"] = "Reklamacija nije pronađena."; return RedirectToAction(nameof(Index)); }
            var vm = ToViewModel(r);
            await PopulateListe(vm);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReklamacijaViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            ModelState.Remove(nameof(vm.RadniciLista));
            ModelState.Remove(nameof(vm.KupciLista));
            ModelState.Remove(nameof(vm.ProizvodiLista));
            ModelState.Remove(nameof(vm.Proizvodi));
            if (!ModelState.IsValid) { await PopulateListe(vm); return View(vm); }
            try
            {
                var entity = new Reklamacija
                {
                    Id = vm.Id, Datum = vm.Datum, TipReklamacije = vm.TipReklamacije,
                    Prioritet = vm.Prioritet, RadnikId = vm.RadnikId
                };
                await _reklamacijaService.UpdateAsync(entity, vm.KupacId, vm.SelectedProizvodiIds);
                TempData["SuccessMessage"] = "Reklamacija uspešno ažurirana!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; await PopulateListe(vm); return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _reklamacijaService.GetByIdAsync(id);
            if (r == null) { TempData["ErrorMessage"] = "Reklamacija nije pronađena."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(r));
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
