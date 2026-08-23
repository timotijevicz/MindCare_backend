using MentalHealth.Data.Models;

namespace MentalHealth.Token
{
    public interface ITokenService
    {
        string CreateToken(Korisnik korisnik);
    }
}
