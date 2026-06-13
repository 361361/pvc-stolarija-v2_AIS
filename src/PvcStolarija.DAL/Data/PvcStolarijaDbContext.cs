using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.DAL.Data
{
    public class PvcStolarijaDbContext : IdentityDbContext<ApplicationUser>
    {
        public PvcStolarijaDbContext(DbContextOptions<PvcStolarijaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kupac> Kupci { get; set; }
        public DbSet<Radnik> Radnici { get; set; }
        public DbSet<Proizvod> Proizvodi { get; set; }
        public DbSet<ProizvodSlika> ProizvodSlike { get; set; }
        public DbSet<Narudzba> Narudzbine { get; set; }
        public DbSet<Reklamacija> Reklamacije { get; set; }
        public DbSet<KupacNarudzba> KupacNarudzbine { get; set; }
        public DbSet<KupacReklamacija> KupacReklamacije { get; set; }
        public DbSet<ReklamacijaProizvod> ReklamacijaProizvodi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KupacNarudzba>()
                .HasKey(kn => new { kn.KupacId, kn.NarudzbaId });
            modelBuilder.Entity<KupacNarudzba>()
                .HasOne(kn => kn.Kupac)
                .WithMany(k => k.KupacNarudzbine)
                .HasForeignKey(kn => kn.KupacId);
            modelBuilder.Entity<KupacNarudzba>()
                .HasOne(kn => kn.Narudzba)
                .WithMany(n => n.KupacNarudzbine)
                .HasForeignKey(kn => kn.NarudzbaId);

            modelBuilder.Entity<KupacReklamacija>()
                .HasKey(kr => new { kr.KupacId, kr.ReklamacijaId });
            modelBuilder.Entity<KupacReklamacija>()
                .HasOne(kr => kr.Kupac)
                .WithMany(k => k.KupacReklamacije)
                .HasForeignKey(kr => kr.KupacId);
            modelBuilder.Entity<KupacReklamacija>()
                .HasOne(kr => kr.Reklamacija)
                .WithMany(r => r.KupacReklamacije)
                .HasForeignKey(kr => kr.ReklamacijaId);

            modelBuilder.Entity<ReklamacijaProizvod>()
                .HasKey(rp => new { rp.ReklamacijaId, rp.ProizvodId });
            modelBuilder.Entity<ReklamacijaProizvod>()
                .HasOne(rp => rp.Reklamacija)
                .WithMany(r => r.ReklamacijaProizvodi)
                .HasForeignKey(rp => rp.ReklamacijaId);
            modelBuilder.Entity<ReklamacijaProizvod>()
                .HasOne(rp => rp.Proizvod)
                .WithMany(p => p.ReklamacijaProizvodi)
                .HasForeignKey(rp => rp.ProizvodId);

            SeedRolesAndAdmin(modelBuilder);
        }

        private void SeedRolesAndAdmin(ModelBuilder modelBuilder)
        {
        }
    }
}
