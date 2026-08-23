using AutoMapper;
using MentalHealth.Data.Models;
using MentalHealth.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MentalHealth.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ========== AUTH ==========
            CreateMap<Korisnik, AuthResponseDTO>()
                .ForMember(dest => dest.Rola, opt => opt.Ignore())
                .ForMember(dest => dest.JeTerapeut, opt => opt.Ignore())
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.IstekTokena, opt => opt.Ignore());

            // ========== KORISNIK ==========
            CreateMap<Korisnik, AuthResponseDTO>()
                 // 1. Mapirajte Id (string) u KorisnikId (string). Nema .ToString() jer je već string!
                 .ForMember(dest => dest.KorisnikId, opt => opt.MapFrom(src => src.Id))

                 // 2. Mapirajte osnovne podatke (imena se poklapaju pa ide automatski, ali je bolje eksplicitno)
                 .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                 .ForMember(dest => dest.Ime, opt => opt.MapFrom(src => src.Ime))
                 .ForMember(dest => dest.Prezime, opt => opt.MapFrom(src => src.Prezime))

                 // 3. Proverite da li je terapeut (ako postoji navigaciono svojstvo Terapeut, znači da jeste)
                 .ForMember(dest => dest.JeTerapeut, opt => opt.MapFrom(src => src.Terapeut != null))

                 // 4. Rola NE POSTOJI na entitetu Korisnik, i ne može se automatski mapirati.
                 // Zato koristimo Ignore(), a ulogu ćete dobiti i postaviti u kontroleru preko UserManager-a.
                 .ForMember(dest => dest.Rola, opt => opt.Ignore())

                 // 5. Token i vreme isteka se generišu tek kad napravite JWT, pa ignorisite ovde.
                 .ForMember(dest => dest.Token, opt => opt.Ignore())
                 .ForMember(dest => dest.IstekTokena, opt => opt.Ignore());

            CreateMap<AzurirajProfilDTO, KorisnickiProfil>();
            CreateMap<KorisnickiProfil, AzurirajProfilDTO>();

            CreateMap<Korisnik, KorisnikPrikazDTO>()
                .ForMember(dest => dest.KorisnikId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Rola, opt => opt.Ignore())
                .ForMember(dest => dest.JeTerapeut, opt => opt.MapFrom(src => src.Terapeut != null))
                .ForMember(dest => dest.OpisProfila, opt => opt.MapFrom(src => src.Profil != null ? src.Profil.OpisProfila : null))
                .ForMember(dest => dest.ProfilnaSlika, opt => opt.MapFrom(src => src.Profil != null ? src.Profil.ProfilnaSlika : null));

            // ========== TERAPEUT ==========
            CreateMap<Terapeut, TerapeutPrikazDTO>()
                .ForMember(dest => dest.Ime, opt => opt.MapFrom(src => src.Korisnik.Ime))
                .ForMember(dest => dest.Prezime, opt => opt.MapFrom(src => src.Korisnik.Prezime))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Korisnik.Email))
                .ForMember(dest => dest.ProfilnaSlika, opt => opt.MapFrom(src => src.Korisnik.Profil != null ? src.Korisnik.Profil.ProfilnaSlika : null));

            CreateMap<AzurirajTerapeutaDTO, Terapeut>();

            // ========== RASPOLOZENJE ==========
            CreateMap<KreirajRaspolozenjeDTO, DnevnoRaspolozenje>();
            CreateMap<DnevnoRaspolozenje, PrikazRaspolozenjaDTO>();

            // ========== DNEVNIK MISLI ==========
            CreateMap<KreirajDnevnikDTO, DnevnikMisli>();
            CreateMap<AzurirajDnevnikDTO, DnevnikMisli>();
            CreateMap<DnevnikMisli, PrikazDnevnikaDTO>()
                .ForMember(dest => dest.KorisnikIme, opt => opt.MapFrom(src =>
                    src.Korisnik != null ? $"{src.Korisnik.Ime} {src.Korisnik.Prezime}" : "Anoniman"));

            // ========== PORUKA ==========
            CreateMap<KreirajPorukuDTO, Poruka>();
            CreateMap<Poruka, PrikazPorukeDTO>()
                .ForMember(dest => dest.ImePosiljaoca, opt => opt.MapFrom(src =>
                    src.Posiljaoc != null ? $"{src.Posiljaoc.Ime} {src.Posiljaoc.Prezime}" : "Nepoznat"))
                .ForMember(dest => dest.ImePrimaoca, opt => opt.MapFrom(src =>
                    src.Primalac != null ? $"{src.Primalac.Ime} {src.Primalac.Prezime}" : "Nepoznat"));

            // ========== SESIJA ==========
            CreateMap<KreirajSesijuDTO, Sesija>();
            CreateMap<Sesija, PrikazSesijeDTO>()
                .ForMember(dest => dest.ImeKlijenta, opt => opt.MapFrom(src =>
                    src.Klijent != null ? $"{src.Klijent.Ime} {src.Klijent.Prezime}" : "Nepoznat"))
                .ForMember(dest => dest.ImeTerapeuta, opt => opt.MapFrom(src =>
                    src.Terapeut != null ? $"{src.Terapeut.Ime} {src.Terapeut.Prezime}" : "Nepoznat"));
            CreateMap<AzurirajSesijuDTO, Sesija>();

            // ========== ZAKAZIVANJE SESIJE ==========
            CreateMap<KreirajZakazivanjeDTO, ZakazivanjeSesije>();
            CreateMap<ZakazivanjeSesije, PrikazZakazivanjaDTO>()
                .ForMember(dest => dest.ImeKlijenta, opt => opt.MapFrom(src =>
                    src.Klijent != null ? $"{src.Klijent.Ime} {src.Klijent.Prezime}" : "Nepoznat"))
                .ForMember(dest => dest.ImeTerapeuta, opt => opt.MapFrom(src =>
                    src.Terapeut != null && src.Terapeut.Korisnik != null ?
                    $"{src.Terapeut.Korisnik.Ime} {src.Terapeut.Korisnik.Prezime}" : "Nepoznat"));

            // ========== PODSETNIK ==========
            CreateMap<KreirajPodsetnikDTO, Podsetnik>();
            CreateMap<Podsetnik, PrikazPodsetnikaDTO>();
            CreateMap<AzurirajPodsetnikDTO, Podsetnik>();

            // ========== EDUKATIVNI SADRZAJ ==========
            CreateMap<KreirajEdukativniSadrzajDTO, EdukativniSadrzaj>();
            CreateMap<EdukativniSadrzaj, PrikazEdukativnogSadrzajaDTO>();

            // ========== SOS KONTAKT ==========
            CreateMap<KreirajSOSKontaktDTO, SOSKontakt>();
            CreateMap<SOSKontakt, PrikazSOSKontaktaDTO>();
            CreateMap<AzurirajSOSKontaktDTO, SOSKontakt>();

            // ========== CILJ ==========
            CreateMap<KreirajCiljDTO, Cilj>();
            CreateMap<Cilj, PrikazCiljaDTO>();
            CreateMap<KreirajKorakCiljaDTO, KorakCilja>();
            CreateMap<KorakCilja, PrikazKorakaCiljaDTO>();

            // ========== NAVIKA ==========
            CreateMap<KreirajNavikuDTO, Navika>();
            CreateMap<Navika, PrikazNavikeDTO>();
            CreateMap<KreirajPracenjeNavikeDTO, PracenjeNavike>();
            CreateMap<PracenjeNavike, PrikazPracenjaNavikeDTO>();


        }
    }
}