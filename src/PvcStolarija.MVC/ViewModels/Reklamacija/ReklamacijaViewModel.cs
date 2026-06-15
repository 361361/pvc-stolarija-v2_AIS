using System.ComponentModel.DataAnnotations;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.MVC.ViewModels.Reklamacija
{
    public class ReklamacijaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Datum je obavezan.")]
        [Display(Name = "Datum")]
        public DateTime Datum { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Tip reklamacije je obavezan.")]
        [Display(Name = "Tip Reklamacije")]
        public TipReklamacije TipReklamacije { get; set; }

        [Required(ErrorMessage = "Prioritet je obavezan.")]
        [Range(0, 100, ErrorMessage = "Prioritet mora biti izmedju 0 i 100.")]
        [Display(Name = "Prioritet")]
        public int Prioritet { get; set; }

        [Required(ErrorMessage = "Radnik je obavezan.")]
        [Display(Name = "Odgovorni Radnik")]
        public int RadnikId { get; set; }

        // Za prikaz
        public string? RadnikImePrezime { get; set; }
        public bool Resena => Prioritet >= 51;

        // Kupac (1 kupac po reklamaciji)
        public int KupacId { get; set; }
        public string? KupacImePrezime { get; set; }

        // Reklamirani proizvodi (samo za Terenska)
        public List<int> SelectedProizvodiIds { get; set; } = new();
        public List<ReklamacijaProizvodViewModel> Proizvodi { get; set; } = new();

        // Za forme
        public List<ReklamacijaRadnikItem> RadniciLista { get; set; } = new();
        public List<ReklamacijaKupacItem> KupciLista { get; set; } = new();
        public List<ReklamacijaProizvodItem> ProizvodiLista { get; set; } = new();
    }

    public class ReklamacijaProizvodViewModel
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Sifra { get; set; }
        public string? PrvaSlika { get; set; }
    }

    public class ReklamacijaRadnikItem  { public int Id { get; set; } public string ImePrezime { get; set; } }
    public class ReklamacijaKupacItem   { public int Id { get; set; } public string ImePrezime { get; set; } public string JMBG { get; set; } }
    public class ReklamacijaProizvodItem { public int Id { get; set; } public string Naziv { get; set; } public string Sifra { get; set; } }
}
