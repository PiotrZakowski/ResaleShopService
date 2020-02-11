using RESTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RESTService.Managers
{
    public interface IAuthService
    {
        string SecretKey { get; set; }

        bool IsTokenValid(string token);
        string GenerateToken(IAuthContainerModel model, bool withExpirationTime = false);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
