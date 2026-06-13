using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.DAL.Models
{
    public class Proizvod
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Naziv { get; set; }

        [Required]
        [MaxLength(50)]
        public string Model { get; set; }

        [Required]
        [MaxLength(20)]
        public string Sifra { get; set; }

        public int? GodinaProizvodnje { get; set; }

        [Required]
        public TipMaterijala TipMaterijala { get; set; }

        [Required]
        public Dostupnost Dostupnost { get; set; }

        public string NazivProizvoda => $"{Naziv} {Model}";

        public ICollection<Narudzba> Narudzbine { get; set; }
        public ICollection<ReklamacijaProizvod> ReklamacijaProizvodi { get; set; }
        public ICollection<ProizvodSlika> Slike { get; set; } = new List<ProizvodSlika>();
    }

    public enum TipMaterijala
    {
        PVC = 0,
        Aluminijum = 1,
        Drvo = 2
    }

    public enum Dostupnost
    {
        Dostupan = 0,
        Nedostupan = 1
    }
}
