namespace PvcStolarija.DAL.Models
{
    public class KupacReklamacija
    {
        public int KupacId { get; set; }
        public Kupac Kupac { get; set; }

        public int ReklamacijaId { get; set; }
        public Reklamacija Reklamacija { get; set; }

        public bool Resena { get; set; }
        public int Prioritet { get; set; }
        public string? Napomena { get; set; }
    }
}
