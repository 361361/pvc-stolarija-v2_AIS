using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PvcStolarija.BLL.Interfaces;
using PvcStolarija.DAL.Models;
using PvcStolarija.MVC.ViewModels.Narudzba;

namespace PvcStolarija.MVC.Controllers
{
    [Authorize]
    public class NarudzbaController : Controller
    {
        private readonly INarudzbaService _narudzbaService;
        public NarudzbaController(INarudzbaService narudzbaService) => _narudzbaService = narudzbaService;

        private static NarudzbaViewModel ToViewModel(Narudzba n) => new()
        {
            Id = n.Id, TipNarudzbe = n.TipNarudzbe, BrojNarudzbe = n.BrojNarudzbe,
            Datum = n.Datum, RadnikId = n.RadnikId, ProizvodId = n.ProizvodId,
            RadnikImePrezime = n.Radnik != null ? $"{n.Radnik.Ime} {n.Radnik.Prezime}" : null,
            ProizvodNaziv = n.Proizvod?.NazivProizvoda,
            KupacNarudzbine = n.KupacNarudzbine?.Select(kn => new KupacNarudzbaViewModel
            {
                KupacId = kn.KupacId,
                KupacImePrezime = $"{kn.Kupac.Ime} {kn.Kupac.Prezime}",
                Potvrdjen = kn.Potvrdjen, Napomena = kn.Napomena
            }).ToList() ?? new(),
            SelectedKupciIds = n.KupacNarudzbine?.Select(kn => kn.KupacId).ToList() ?? new()
        };

        private async Task PopulateListe(NarudzbaViewModel vm)
        {
            vm.RadniciLista = (await _narudzbaService.GetAllRadniciAsync())
                .Select(r => new RadnikSelectItem { Id = r.Id, ImePrezime = $"{r.Ime} {r.Prezime}" }).ToList();
            vm.ProizvodiLista = (await _narudzbaService.GetAllProizvodiAsync())
                .Select(p => new ProizvodSelectItem { Id = p.Id, Naziv = p.NazivProizvoda }).ToList();
            vm.SviKupci = (await _narudzbaService.GetAllKupciAsync())
                .Select(k => new KupacSelectItem { Id = k.Id, ImePrezime = $"{k.Ime} {k.Prezime}", JMBG = k.JMBG }).ToList();
        }

        public async Task<IActionResult> Index()
        {
            var narudzbine = await _narudzbaService.GetAllAsync();
            return View(narudzbine.Select(ToViewModel).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var n = await _narudzbaService.GetByIdAsync(id);
            if (n == null) { TempData["ErrorMessage"] = "Narudžba nije pronađena."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(n));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            var vm = new NarudzbaViewModel();
            await PopulateListe(vm);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NarudzbaViewModel vm)
        {
            ModelState.Remove(nameof(vm.RadniciLista));
            ModelState.Remove(nameof(vm.ProizvodiLista));
            ModelState.Remove(nameof(vm.SviKupci));
            ModelState.Remove(nameof(vm.KupacNarudzbine));
            if (!ModelState.IsValid) { await PopulateListe(vm); return View(vm); }
            try
            {
                var entity = new Narudzba
                {
                    TipNarudzbe = vm.TipNarudzbe, BrojNarudzbe = vm.BrojNarudzbe,
                    Datum = vm.Datum, RadnikId = vm.RadnikId, ProizvodId = vm.ProizvodId
                };
                await _narudzbaService.AddAsync(entity, vm.SelectedKupciIds ?? new());
                TempData["SuccessMessage"] = "Narudžba je uspešno dodata!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; await PopulateListe(vm); return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var n = await _narudzbaService.GetByIdAsync(id);
            if (n == null) { TempData["ErrorMessage"] = "Narudžba nije pronađena."; return RedirectToAction(nameof(Index)); }
            var vm = ToViewModel(n);
            await PopulateListe(vm);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NarudzbaViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            ModelState.Remove(nameof(vm.RadniciLista));
            ModelState.Remove(nameof(vm.ProizvodiLista));
            ModelState.Remove(nameof(vm.SviKupci));
            ModelState.Remove(nameof(vm.KupacNarudzbine));
            if (!ModelState.IsValid) { await PopulateListe(vm); return View(vm); }
            try
            {
                var entity = new Narudzba
                {
                    Id = vm.Id, TipNarudzbe = vm.TipNarudzbe, BrojNarudzbe = vm.BrojNarudzbe,
                    Datum = vm.Datum, RadnikId = vm.RadnikId, ProizvodId = vm.ProizvodId
                };
                var kupacData = new List<(int KupacId, bool Potvrdjen, string? Napomena)>();
                foreach (var kupacId in vm.SelectedKupciIds ?? new())
                {
                    bool potvrdjen = Request.Form[$"potvrdjen_{kupacId}"] == "on";
                    string? napomena = Request.Form[$"napomene_{kupacId}"];
                    kupacData.Add((kupacId, potvrdjen, napomena));
                }
                await _narudzbaService.UpdateAsync(entity, kupacData);
                TempData["SuccessMessage"] = "Narudžba je uspešno ažurirana!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) { TempData["ErrorMessage"] = $"Greška: {ex.Message}"; await PopulateListe(vm); return View(vm); }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var n = await _narudzbaService.GetByIdAsync(id);
            if (n == null) { TempData["ErrorMessage"] = "Narudžba nije pronađena."; return RedirectToAction(nameof(Index)); }
            return View(ToViewModel(n));
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
