using System.ComponentModel.DataAnnotations;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.MVC.ViewModels.Narudzba
{
    public class NarudzbaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tip narudzbe je obavezan.")]
        [Display(Name = "Tip Narudzbe")]
        public TipNarudzbe TipNarudzbe { get; set; }

        [Required(ErrorMessage = "Broj narudzbe je obavezan.")]
        [Range(1, 9999, ErrorMessage = "Broj narudzbe mora biti izmedju 1 i 9999.")]
        [Display(Name = "Broj Narudzbe")]
        public int BrojNarudzbe { get; set; }

        [Display(Name = "Datum")]
        public DateOnly? Datum { get; set; }

        [Display(Name = "Radnik")]
        public int? RadnikId { get; set; }

        [Display(Name = "Proizvod")]
        public int? ProizvodId { get; set; }

        // Za prikaz
        public string? RadnikImePrezime { get; set; }
        public string? ProizvodNaziv { get; set; }
        public List<KupacNarudzbaViewModel> KupacNarudzbine { get; set; } = new();

        // Za forme — dostupne liste
        public List<RadnikSelectItem> RadniciLista { get; set; } = new();
        public List<ProizvodSelectItem> ProizvodiLista { get; set; } = new();
        public List<KupacSelectItem> SviKupci { get; set; } = new();
        public List<int> SelectedKupciIds { get; set; } = new();
    }

    public class KupacNarudzbaViewModel
    {
        public int KupacId { get; set; }
        public string KupacImePrezime { get; set; }
        public bool Potvrdjen { get; set; }
        public string? Napomena { get; set; }
    }

    public class RadnikSelectItem { public int Id { get; set; } public string ImePrezime { get; set; } }
    public class ProizvodSelectItem { public int Id { get; set; } public string Naziv { get; set; } }
    public class KupacSelectItem  { public int Id { get; set; } public string ImePrezime { get; set; } public string JMBG { get; set; } }
}
