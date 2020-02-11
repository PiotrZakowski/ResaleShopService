using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using RESTService.Managers;
using RESTService.Models;

namespace RESTService.Controllers
{
    internal class MyAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            HttpRequestMessage request = actionContext.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            if (authorization == null)
            {
                actionContext.Response = new AuthenticationFailureResult("Authorization is needed.", request).Create();
                return;
            }

            if (authorization.Scheme != "Bearer")
            {
                actionContext.Response = new AuthenticationFailureResult("Not supported authorization method.", request).Create();
                return;
            }

            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                actionContext.Response = new AuthenticationFailureResult("Authorization credentials is empty.", request).Create();
                return;
            }

            string token = request.Headers.Authorization.Parameter;

            JWTService service = new JWTService(DefaultSecretKey.key);
            if (!service.IsTokenValid(token))
            {
                actionContext.Response = new AuthenticationFailureResult("Token is not valid or expired.", request).Create();
                return;
            }

            string tokenType; //username, password,
            List<string> userRoles;
            List<Claim> tokenClaims = service.GetTokenClaims(token).ToList();
            //username = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.Username)).Value;
            //password = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.Password)).Value;
            userRoles = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.Roles)).Value.Split(',').ToList();
            tokenType = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.TokenType)).Value;

            if (tokenType != MyTokenTypes.BearerToken)
            {
                actionContext.Response = new AuthenticationFailureResult("Refresh Token was used instead of Bearer Token.", request).Create();
                return;
            }

            if (request.Method.ToString().Equals(HttpMethod.Delete.ToString()))
                if (!userRoles.Any(e => e == "Manager"))
                {
                    actionContext.Response = new AuthenticationFailureResult("Action forbidden. User do not have necessarily permissions.", request).Create();
                    return;
                }
        }
    }
}