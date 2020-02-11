using Newtonsoft.Json;
using RESTService.Managers;
using RESTService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RESTService.Controllers
{
    public class UsersController : ApiController
    {
        //POST /Register
        /// <summary>
        /// Registering new user.
        /// </summary>
        /// <param name="user">User username and password</param>
        /// <returns>Http result code</returns>
        [Route("~/api/Users/Register")]
        public IHttpActionResult Register([FromBody]Users user)
        {
            System.Web.Http.Results.StatusCodeResult status;
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                #region Validation

                #region checkIfUserIsNull
                if (user == null)
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.Conflict, this);
                    return status;
                }
                #endregion

                #region checkIfUsernameIsTaken
                bool checkIfUsernameIsTaken = entities.Users
                    .Any(e => e.Username == user.Username);

                if (checkIfUsernameIsTaken)
                {
                    status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.Conflict, this);
                    return status;
                }
                #endregion

                #endregion

                CryptoService cryptoService = new CryptoService();
                string hashedPassword = cryptoService.GetHashedString(user.Password);

                Users newUser = new Users()
                {
                    Username = user.Username,
                    Password = hashedPassword,
                    GoogleId = user.GoogleId
                };
                entities.Users.Add(newUser);
                entities.SaveChanges();

                Users dbUser = entities.Users
                    .Where(e => e.Username == user.Username)
                    .First();

                UserRoles employeeRole = entities.UserRoles
                    .Where(e => e.RoleName == "Employee")
                    .First();

                Workplaces newWorkplace = new Workplaces()
                {
                    UserId = dbUser.Id,
                    UserRoleId = employeeRole.Id
                };
                entities.Workplaces.Add(newWorkplace);
                entities.SaveChanges();

                status = new System.Web.Http.Results.StatusCodeResult(HttpStatusCode.Created, this);
                return status;
            }
        }

        //POST /Login
        /// <summary>
        /// Logging in.
        /// </summary>
        /// <param name="user">User username and password</param>
        /// <returns>Refresh token</returns>
        [Route("~/api/Users/LogIn")]
        public TokenModel LogIn([FromBody]Users user)
        {
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                #region Validation

                #region checkIfUserIsNull
                if (user == null)
                {
                    return null;
                }
                #endregion

                #region checkIfUserExist
                bool checkIfUserExist = entities.Users
                    .Any(e => e.Username == user.Username);

                if (!checkIfUserExist)
                    return null;
                #endregion

                string dbUserHashedPassword = entities.Users
                    .Where(e => e.Username == user.Username)
                    .Select(e => e.Password)
                    .First();
                CryptoService cryptoService = new CryptoService();

                #region checkIfUserPasswordMatches
                bool checkIfUserPasswordMatches = cryptoService.CompareStringToHash(user.Password, dbUserHashedPassword);

                if (!checkIfUserPasswordMatches)
                    return null;
                #endregion

                #endregion

                Users dbUser = entities.Users
                    .First(e => e.Username == user.Username);

                List<string> userRoles = entities.Workplaces
                    .Where(e => e.UserId == dbUser.Id)
                    .Select(e => e.UserRoles.RoleName)
                    .ToList();

                JWTContainerModel newRefreshTokenJWTContainerModel = JWTContainerModel.GetUserJWTContainerModel(
                    user.Username, user.Password, userRoles, MyTokenTypes.RefreshToken);
                JWTService serviceJWT = new JWTService(DefaultSecretKey.key);
                string newRefreshToken = serviceJWT.GenerateToken(newRefreshTokenJWTContainerModel);

                Users newUser = new Users()
                {
                    Id = dbUser.Id,
                    Username = dbUser.Username,
                    Password = dbUser.Password,
                    RefreshToken = newRefreshToken,
                    GoogleId = dbUser.GoogleId
                };

                entities.Users.AddOrUpdate(newUser);
                entities.SaveChanges();

                return new TokenModel(newRefreshToken);
            }
        }
        
        //POST /StartSession
        /// <summary>
        /// Starting new session.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>Bearer Token</returns>
        [Route("~/api/Users/StartSession")]
        public TokenModel StartSession([FromBody]TokenModel refreshToken)
        {
            using (IUMdbEntities entities = new IUMdbEntities())
            {
                #region Validation

                #region CheckIfRefreshTokenIsNull
                if(refreshToken == null)
                {
                    return null;
                }
                #endregion

                JWTService serviceJWT = new JWTService(DefaultSecretKey.key);

                #region checkIfTokenIsValid
                if (!serviceJWT.IsTokenValid(refreshToken.Token))
                    return null;
                #endregion

                string username, password, tokenType;
                List<string> userRoles;
                List<Claim> tokenClaims = serviceJWT.GetTokenClaims(refreshToken.Token).ToList();
                username = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.Username)).Value;
                password = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.Password)).Value;
                userRoles = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.Roles)).Value.Split(',').ToList();
                tokenType = tokenClaims.FirstOrDefault(e => e.Type.Equals(MyClaimsTypes.TokenType)).Value;

                #region checkTokenType
                if (!tokenType.Equals(MyTokenTypes.RefreshToken))
                    return null;
                #endregion

                #region checkIfRefreshTokenMatches
                bool checkIfRefreshTokenMatches = entities.Users
                    .Any(e => e.Username == username && e.RefreshToken == refreshToken.Token);

                if (!checkIfRefreshTokenMatches)
                    return null;
                #endregion

                string dbUserHashedPassword = entities.Users
                    .Where(e => e.Username == username)
                    .Select(e => e.Password)
                    .First();
                CryptoService cryptoService = new CryptoService();

                #region checkIfUserPasswordMatches
                bool checkIfUserPasswordMatches = cryptoService.CompareStringToHash(password, dbUserHashedPassword);

                if (!checkIfUserPasswordMatches)
                    return null;
                #endregion 

                #endregion

                Users dbUser = entities.Users
                    .Where(e => e.Username == username)
                    .First();
                    
                JWTContainerModel newBearerTokenJWTContainerModel = JWTContainerModel.GetUserJWTContainerModel(
                    username, password, userRoles, MyTokenTypes.BearerToken);
                string newBearerToken = serviceJWT.GenerateToken(newBearerTokenJWTContainerModel, true);

                Users newUser = new Users()
                {
                    Id = dbUser.Id,
                    Username = dbUser.Username,
                    Password = dbUser.Password,
                    RefreshToken = dbUser.RefreshToken,
                    BearerToken = newBearerToken,
                    GoogleId = dbUser.GoogleId
                };

                entities.Users.AddOrUpdate(newUser);
                entities.SaveChanges();

                return new TokenModel(newBearerToken);
            }
        }

        //POST /LogInUsingGoogleAccount
        /// <summary>
        /// Logging in and starting new session using Google identity provider
        /// </summary>
        /// <param name="googleUser">Google user credentials</param>
        /// <param name="idToken">User Id Token from Google Authorization</param>
        /// <returns>Refresh Token</returns>
        [Route("~/api/Users/LogInUsingGoogleAccount")]
        public TokenModel LogInUsingGoogleAccount([FromBody]GoogleUserModel googleUser, string idToken)
        {
            //Validate
            string googleValidateURL = "https://oauth2.googleapis.com/tokeninfo?id_token=" + idToken;

            HttpClient client = new HttpClient();
            var response = client.GetAsync(googleValidateURL).GetAwaiter().GetResult();
            string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
                return null;

            GoogleIdTokenClaimsModel IdTokenClaims = JsonConvert.DeserializeObject<GoogleIdTokenClaimsModel>(responseString);

            if (IdTokenClaims.aud != ConfigurationManager.AppSettings["GoogleAndroidAppClientId"]
                || IdTokenClaims.sub != googleUser.Id)
                return null;


            //Check if exist in database
            Users user = new Users()
            {
                Username = "GoogleUser" + googleUser.Id,
                Password = googleUser.Email + ConfigurationManager.AppSettings["GoogleUsersSalt"],
                GoogleId = googleUser.Id
            };

            Register(user);

            //Grant bearer token
            TokenModel bearerToken = StartSession(LogIn(user));

            return bearerToken;
        }
    }
}
