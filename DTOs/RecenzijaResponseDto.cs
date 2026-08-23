using System;

namespace MentalHealth.Data.DTOs
{
    public class RecenzijaResponseDto
    {
        public int Id { get; set; }
        public string Tekst { get; set; }
        public int Ocena { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public string KorisnikIme { get; set; }
        public string KorisnikId { get; set; }
        public string Status { get; set; }
    }
}
