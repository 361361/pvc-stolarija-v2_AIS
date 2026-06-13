using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.DAL.Models
{
    public class Kupac
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

        [Required]
        public DateTime DatumRegistracije { get; set; }

        public string? ProfilnaSlika { get; set; }

        public string ImePrezime => $"{Ime} {Prezime}";

        public ICollection<KupacNarudzba> KupacNarudzbine { get; set; }
        public ICollection<KupacReklamacija> KupacReklamacije { get; set; }
    }
}
