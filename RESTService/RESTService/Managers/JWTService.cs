using Microsoft.IdentityModel.Tokens;
using RESTService.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace RESTService.Managers
{
    public class JWTService : IAuthService
    {
        public string SecretKey { get; set; }

        public JWTService(string secretKey)
        {
            SecretKey = secretKey;
        }

        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(this.SecretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey(),
                ClockSkew = TimeSpan.Zero
        };
        }

        public bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = this.GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, 
                    tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public string GenerateToken(IAuthContainerModel model, bool withExpirationTime=false)
        {
            if (model == null || model.Claims == null || model.Claims.Length == 0)
                throw new ArgumentException("Arguments to create token are not valid");

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(model.Claims),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), model.SecurityAlgorithm)
            };

            if (withExpirationTime)
                securityTokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(model.ExpireMinutes));

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            string token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return token;
        }

        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();
            
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token,
                    tokenValidationParameters, out SecurityToken validatedToken);
                return tokenValid.Claims;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}