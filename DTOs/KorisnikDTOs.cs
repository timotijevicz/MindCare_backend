using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class AzurirajProfilDTO
    {
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? OpisProfila { get; set; }
        public string? ProfilnaSlika { get; set; }
        public string? PreferiraniSat { get; set; }
        public bool PrimiMotivacionuPoruku { get; set; }
        public bool PrimiPodsetnik { get; set; }
        public string? Cilj { get; set; }
    }

    public class AktivnostNalogaDTO
    {
        public bool Aktivan { get; set; }
    }

    public class KorisnikPrikazDTO
    {
        public string KorisnikId { get; set; }
        public string Email { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public bool AnonimnaRegistracija { get; set; }
        public bool AktivnaNalog { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public DateTime ZadnjaAktivnost { get; set; }
        public string Rola { get; set; }
        public bool JeTerapeut { get; set; }
        public string OpisProfila { get; set; }
        public string ProfilnaSlika { get; set; }
    }

    public class TerapeutPrikazDTO
    {
        public int TerapeutId { get; set; }
        public string KorisnikId { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Zvanje { get; set; }
        public string Licenca { get; set; }
        public string OpisPrakse { get; set; }
        public bool Dostupan { get; set; }
        public decimal SatnicaKonzultacije { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public string ProfilnaSlika { get; set; }
    }

    public class AzurirajTerapeutaDTO
    {
        public string Zvanje { get; set; }
        public string Licenca { get; set; }
        public string OpisPrakse { get; set; }
        public bool Dostupan { get; set; }
        public decimal SatnicaKonzultacije { get; set; }
    }
}