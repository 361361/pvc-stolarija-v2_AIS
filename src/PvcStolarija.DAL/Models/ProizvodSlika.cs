using System.ComponentModel.DataAnnotations;

namespace PvcStolarija.DAL.Models
{
    public class ProizvodSlika
    {
        public int Id { get; set; }

        public int ProizvodId { get; set; }
        public Proizvod Proizvod { get; set; }

        [Required]
        public string PutanjaDoSlike { get; set; }

        public string? Opis { get; set; }
    }
}
