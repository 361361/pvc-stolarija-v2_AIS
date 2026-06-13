namespace PvcStolarija.DAL.Models
{
    public class ReklamacijaProizvod
    {
        public int ReklamacijaId { get; set; }
        public Reklamacija Reklamacija { get; set; }

        public int ProizvodId { get; set; }
        public Proizvod Proizvod { get; set; }
    }
}
