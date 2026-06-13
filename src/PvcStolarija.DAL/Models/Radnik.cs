using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.DAL.Models
{
    public class Radnik
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Ime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Prezime { get; set; }

        [Required]
        [MaxLength(13)]
        public string JMBG { get; set; }

        public string? Telefon { get; set; }
        public string? Email { get; set; }

        [MaxLength(30)]
        public string? BrojUgovora { get; set; }

        [Required]
        [Range(0, 50)]
        public int GodineRadnogIskustva { get; set; }

        public string? ProfilnaSlika { get; set; }

        public string ImePrezime => $"{Ime} {Prezime}";

        public ICollection<Narudzba> Narudzbine { get; set; }
        public ICollection<Reklamacija> Reklamacije { get; set; }
    }
}
