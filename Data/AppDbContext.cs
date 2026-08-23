using MentalHealth.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Data
{
    public class AppDbContext : IdentityDbContext<Korisnik, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet-ovi za sve entitete
        public DbSet<KorisnickiProfil> KorisnickiProfili { get; set; }
        public DbSet<Terapeut> Terapeuti { get; set; }
        public DbSet<DnevnoRaspolozenje> DnevnaRaspolozenja { get; set; }
        public DbSet<DnevnikMisli> DnevnikMisli { get; set; }
        public DbSet<Poruka> Poruke { get; set; }
        public DbSet<Sesija> Sesije { get; set; }
        public DbSet<ZakazivanjeSesije> ZakazivanjaSesija { get; set; }
        public DbSet<Podsetnik> Podsetnici { get; set; }
        public DbSet<EdukativniSadrzaj> EdukativniSadrzaj { get; set; }
        public DbSet<SOSKontakt> SOSKontakti { get; set; }
        public DbSet<Cilj> Ciljevi { get; set; }
        public DbSet<KorakCilja> KoraciCiljeva { get; set; }
        public DbSet<Navika> Navike { get; set; }
        public DbSet<PracenjeNavike> PracenjaNavika { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== PRIMARY KEYS FOR ENTITIES WITH CUSTOM ID PROPERTY NAMES ==========
            modelBuilder.Entity<KorisnickiProfil>().HasKey(kp => kp.ProfilId);
            modelBuilder.Entity<DnevnoRaspolozenje>().HasKey(dr => dr.RaspolozenjeId);
            modelBuilder.Entity<DnevnikMisli>().HasKey(dm => dm.BeleskaId);
            modelBuilder.Entity<EdukativniSadrzaj>().HasKey(es => es.SadrzajId);
            modelBuilder.Entity<KorakCilja>().HasKey(kc => kc.KorakId);
            modelBuilder.Entity<PracenjeNavike>().HasKey(pn => pn.PracenjeId);

            // ========== KORISNICKI PROFIL ==========
            modelBuilder.Entity<KorisnickiProfil>()
                .HasOne(kp => kp.Korisnik)
                .WithOne(k => k.Profil)
                .HasForeignKey<KorisnickiProfil>(kp => kp.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== TERAPEUT ==========
            modelBuilder.Entity<Terapeut>().Property(t => t.SatnicaKonzultacije).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Terapeut>()
                .HasOne(t => t.Korisnik)
                .WithOne(k => k.Terapeut)
                .HasForeignKey<Terapeut>(t => t.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== DNEVNO RASPOLOZENJE ==========
            modelBuilder.Entity<DnevnoRaspolozenje>()
                .HasOne(dr => dr.Korisnik)
                .WithMany(k => k.Raspolozenja)
                .HasForeignKey(dr => dr.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== DNEVNIK MISLI ==========
            modelBuilder.Entity<DnevnikMisli>()
                .HasOne(dm => dm.Korisnik)
                .WithMany(k => k.DnevnikMisli)
                .HasForeignKey(dm => dm.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== PORUKA ==========
            modelBuilder.Entity<Poruka>()
                .HasOne(p => p.Posiljaoc)
                .WithMany(k => k.PoslatePoruke)
                .HasForeignKey(p => p.PosiljaocId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Poruka>()
                .HasOne(p => p.Primalac)
                .WithMany(k => k.PrimljenePoruke)
                .HasForeignKey(p => p.PrimalacId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Poruka>()
                .HasOne(p => p.Sesija)
                .WithMany(s => s.Poruke)
                .HasForeignKey(p => p.SesijaId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== SESIJA ==========
            modelBuilder.Entity<Sesija>()
                .HasOne(s => s.Klijent)
                .WithMany(k => k.SesijeKaoKlijent)
                .HasForeignKey(s => s.KlijentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Sesija>()
                .HasOne(s => s.Terapeut)
                .WithMany(k => k.SesijeKaoTerapeut)
                .HasForeignKey(s => s.TerapeutId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========== ZAKAZIVANJE SESIJE ==========
            modelBuilder.Entity<ZakazivanjeSesije>()
                .HasOne(zs => zs.Klijent)
                .WithMany()
                .HasForeignKey(zs => zs.KlijentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ZakazivanjeSesije>()
                .HasOne(zs => zs.Terapeut)
                .WithMany(t => t.ZakazaneSesije)
                .HasForeignKey(zs => zs.TerapeutId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========== PODSETNIK ==========
            modelBuilder.Entity<Podsetnik>()
                .HasOne(p => p.Korisnik)
                .WithMany(k => k.Podsetnici)
                .HasForeignKey(p => p.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== SOS KONTAKT ==========
            modelBuilder.Entity<SOSKontakt>()
                .HasOne(s => s.Korisnik)
                .WithMany(k => k.SOSKontakti)
                .HasForeignKey(s => s.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== CILJ ==========
            modelBuilder.Entity<Cilj>()
                .HasOne(c => c.Korisnik)
                .WithMany(k => k.Ciljevi)
                .HasForeignKey(c => c.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== KORAK CILJA ==========
            modelBuilder.Entity<KorakCilja>()
                .HasOne(kc => kc.Cilj)
                .WithMany(c => c.Koraci)
                .HasForeignKey(kc => kc.CiljId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== NAVIKA ==========
            modelBuilder.Entity<Navika>()
                .HasOne(n => n.Korisnik)
                .WithMany(k => k.Navike)
                .HasForeignKey(n => n.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== PRACENJE NAVIKE ==========
            modelBuilder.Entity<PracenjeNavike>()
                .HasOne(pn => pn.Navika)
                .WithMany(n => n.Pracenja)
                .HasForeignKey(pn => pn.NavikaId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== RECENZIJA ==========
            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.Korisnik)
                .WithMany(k => k.Recenzije)
                .HasForeignKey(r => r.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== SEED PODACI ZA ROLE ==========
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "Terapeut",
                    NormalizedName = "TERAPEUT"
                },
                new IdentityRole
                {
                    Id = "3",
                    Name = "Klijent",
                    NormalizedName = "KLIJENT"
                }
            );
        }
    }
}