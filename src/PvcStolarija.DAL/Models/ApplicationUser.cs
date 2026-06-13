using Microsoft.AspNetCore.Identity;

namespace PvcStolarija.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? ProfilnaSlika { get; set; }

        public int? KupacId { get; set; }
        public Kupac? Kupac { get; set; }
    }
}
