namespace PvcStolarija.DAL.Models
{
    public class KupacNarudzba
    {
        public int KupacId { get; set; }
        public Kupac Kupac { get; set; }

        public int NarudzbaId { get; set; }
        public Narudzba Narudzba { get; set; }

        public bool Potvrdjen { get; set; }
        public string? Napomena { get; set; }
    }
}
