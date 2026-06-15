using System.ComponentModel.DataAnnotations;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.MVC.ViewModels.Proizvod
{
    public class ProizvodViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [MaxLength(50)]
        [Display(Name = "Naziv")]
        public string Naziv { get; set; }

        [Required(ErrorMessage = "Model je obavezan.")]
        [MaxLength(50)]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Sifra je obavezna.")]
        [MaxLength(20)]
        [Display(Name = "Sifra")]
        public string Sifra { get; set; }

        [Display(Name = "Godina Proizvodnje")]
        [Range(1990, 2100)]
        public int? GodinaProizvodnje { get; set; }

        [Required(ErrorMessage = "Tip materijala je obavezan.")]
        [Display(Name = "Tip Materijala")]
        public TipMaterijala TipMaterijala { get; set; }

        [Required(ErrorMessage = "Dostupnost je obavezna.")]
        [Display(Name = "Dostupnost")]
        public Dostupnost Dostupnost { get; set; }

        // Slike — za prikaz
        public List<ProizvodSlikaViewModel> Slike { get; set; } = new();

        // Slike — za upload
        public List<IFormFile>? NoveSlike { get; set; }
        public List<string>? OpisiNovihSlika { get; set; }

        public string NazivProizvoda => $"{Naziv} {Model}";
    }

    public class ProizvodSlikaViewModel
    {
        public int Id { get; set; }
        public string PutanjaDoSlike { get; set; }
        public string? Opis { get; set; }
    }
}
