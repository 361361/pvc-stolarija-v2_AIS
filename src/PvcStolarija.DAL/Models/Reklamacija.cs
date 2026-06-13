using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.DAL.Models
{
    public class Reklamacija
    {
        public int Id { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        [Required]
        public TimeSpan VremePocetka { get; set; }

        [Required]
        public TipReklamacije TipReklamacije { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Prioritet mora biti između 0 i 100")]
        public int Prioritet { get; set; }

        public bool Resena => Prioritet >= 51;

        [Required]
        public int RadnikId { get; set; }
        public Radnik? Radnik { get; set; }

        public ICollection<KupacReklamacija> KupacReklamacije { get; set; } = new List<KupacReklamacija>();
        public ICollection<ReklamacijaProizvod> ReklamacijaProizvodi { get; set; } = new List<ReklamacijaProizvod>();
    }

    public enum TipReklamacije
    {
        Pisana = 0,
        Terenska = 1
    }
}
