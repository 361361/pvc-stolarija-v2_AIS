using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.MVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        [Display(Name = "Korisničko ime")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Neispravna email adresa")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [StringLength(100, ErrorMessage = "{0} mora biti minimum {2} karaktera", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi lozinku")]
        [Compare("Password", ErrorMessage = "Lozinke se ne poklapaju")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Ime")]
        public string Ime { get; set; }

        [Required]
        [Display(Name = "Prezime")]
        public string Prezime { get; set; }

        // Samo za kupace
        public int? KupacId { get; set; }
    }
}