using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace RESTService.Models
{
    public static class MyClaimsTypes
    {
        public const string Username = "username";
        public const string Password = "password";
        public const string Roles = "userRoles";
        public const string TokenType = "tokenType";
        public const string Salt = "salt";
    }

    public static class MyTokenTypes
    {
        public const string RefreshToken = "refreshToken";
        public const string BearerToken = "bearerToken";
    }

    public static class DefaultSecretKey
    {
        public static string key
        {
            get
            {
                return ConfigurationManager.AppSettings["JWTSecretKey"];
            }
        }
    }

    public class JWTContainerModel : IAuthContainerModel
    {
        public int ExpireMinutes { get; set; } = 2 * 60;/* 2*60 */
        public string SecretKey { get; set; } = DefaultSecretKey.key;
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;

        public Claim[] Claims { get; set; }

        public static JWTContainerModel GetUserJWTContainerModel(string username, string password, List<string> roles, string tokenType)
        {
            string rolesConcat = string.Join(",", roles.ToArray());
            Random rnd = new Random();

            return new JWTContainerModel()
            {
                Claims = new Claim[]
                {
                    new Claim(MyClaimsTypes.Username, username),
                    new Claim(MyClaimsTypes.Password, password),
                    new Claim(MyClaimsTypes.Roles, rolesConcat),
                    new Claim(MyClaimsTypes.TokenType, tokenType),
                    new Claim(MyClaimsTypes.Salt, rnd.Next(int.MinValue,int.MaxValue).ToString())
                }
            };
        }
    }
}