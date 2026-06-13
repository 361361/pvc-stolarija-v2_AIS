using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.DAL.Models
{
    public class Narudzba
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public TipNarudzbe TipNarudzbe { get; set; }

        [Required]
        [Range(1, 9999, ErrorMessage = "Broj narudžbe mora biti između 1 i 9999")]
        public int BrojNarudzbe { get; set; }

        public DateOnly? Datum { get; set; }

        public int? RadnikId { get; set; }
        public Radnik? Radnik { get; set; }

        public int? ProizvodId { get; set; }
        public Proizvod? Proizvod { get; set; }

        public ICollection<KupacNarudzba> KupacNarudzbine { get; set; }
    }

    public enum TipNarudzbe
    {
        Online = 0,
        Licno = 1
    }
}
