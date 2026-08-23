using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajPodsetnikDTO
    {
        [Required(ErrorMessage = "Tip podsetnika je obavezan")]
        public string Tip { get; set; } // "Raspoloženje", "Dnevnik", "Meditacija", "Disanje"

        [Required(ErrorMessage = "Tekst podsetnika je obavezan")]
        public string Tekst { get; set; }

        [Required(ErrorMessage = "Vreme podsetnika je obavezno")]
        public string VremePodsetnika { get; set; } // HH:mm format

        public bool Aktivan { get; set; } = true;
    }

    public class PrikazPodsetnikaDTO
    {
        public int PodsetnikId { get; set; }
        public string Tip { get; set; }
        public string Tekst { get; set; }
        public bool Aktivan { get; set; }
        public string VremePodsetnika { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }

    public class AzurirajPodsetnikDTO
    {
        public string Tip { get; set; }
        public string Tekst { get; set; }
        public bool Aktivan { get; set; }
        public string VremePodsetnika { get; set; }
    }
}