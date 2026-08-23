using Microsoft.AspNetCore.Identity;

namespace MentalHealth.Data.Models
{
    public class Korisnik : IdentityUser
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public bool AnonimnaRegistracija { get; set; }
        public bool AktivnaNalog { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public DateTime ZadnjaAktivnost { get; set; }

        // Navigaciona svojstva
        public virtual KorisnickiProfil Profil { get; set; }
        public virtual Terapeut Terapeut { get; set; }
        public virtual List<DnevnoRaspolozenje> Raspolozenja { get; set; } = new();
        public virtual List<DnevnikMisli> DnevnikMisli { get; set; } = new();
        public virtual List<Poruka> PoslatePoruke { get; set; } = new();
        public virtual List<Poruka> PrimljenePoruke { get; set; } = new();
        public virtual List<Sesija> SesijeKaoKlijent { get; set; } = new();
        public virtual List<Sesija> SesijeKaoTerapeut { get; set; } = new();
        public virtual List<Podsetnik> Podsetnici { get; set; } = new();
        public virtual List<SOSKontakt> SOSKontakti { get; set; } = new();
        public virtual List<Cilj> Ciljevi { get; set; } = new();
        public virtual List<Navika> Navike { get; set; } = new();
        public virtual List<Recenzija> Recenzije { get; set; } = new();
    }
}