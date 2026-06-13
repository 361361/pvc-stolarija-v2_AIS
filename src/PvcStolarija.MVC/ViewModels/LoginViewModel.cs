using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.MVC.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        [Display(Name = "Korisničko ime")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; }

        [Display(Name = "Zapamti me")]
        public bool RememberMe { get; set; }
    }
}