using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.MVC.ViewModels.Radnik
{
    public class RadnikViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime je obavezno.")]
        [MaxLength(50)]
        [Display(Name = "Ime")]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [MaxLength(50)]
        [Display(Name = "Prezime")]
        public string Prezime { get; set; }

        [Required(ErrorMessage = "JMBG je obavezan.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG mora imati tacno 13 cifara.")]
        [Display(Name = "JMBG")]
        public string JMBG { get; set; }

        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        [EmailAddress(ErrorMessage = "Neispravna email adresa.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [MaxLength(30)]
        [Display(Name = "Broj Ugovora")]
        public string? BrojUgovora { get; set; }

        [Required(ErrorMessage = "Godine radnog iskustva su obavezne.")]
        [Range(0, 50, ErrorMessage = "Godine iskustva moraju biti izmedju 0 i 50.")]
        [Display(Name = "Godine Radnog Iskustva")]
        public int GodineRadnogIskustva { get; set; }

        [Display(Name = "Profilna Slika")]
        public string? ProfilnaSlika { get; set; }

        public IFormFile? ProfilnaSlikaFajl { get; set; }

        public string ImePrezime => $"{Ime} {Prezime}";
    }
}
