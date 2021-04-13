using System.Threading.Tasks;
using QuickNote_Models.Token;

namespace QuickNote_Services.Token
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(TokenRequest model);
    }
}