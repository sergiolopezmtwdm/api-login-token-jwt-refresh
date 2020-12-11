using API_TAREA.Models;
using APIUsers.Library.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_TAREA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        //readonly UserContext userContext;
        readonly ITokenService tokenService;
        readonly IConfiguration _configuration;
        public TokenController(IConfiguration configuration, ITokenService tokenService)
        {
            //this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _configuration = configuration;
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
            {
                return BadRequest("Invalid client request");
            }
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            //var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken, new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("SecretKey"))));
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
            //En lugar de usar la conexión por contexto, usamos la conexión mediante los servicios ya creados.
            //var user = userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
            //Conexión mediante clase Login
            var ConnectionStringLocal = _configuration.GetValue<string>("ServidorLocal");            
            using (IUser User = Factorizador.CrearConexionServicio(APIUsers.Library.Models.ConnectionType.MSSQL, ConnectionStringLocal))
            {
                APIUsers.Library.Models.User user = User.GetUser(username);
                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    return BadRequest("Invalid client request");
                }
                //var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
                var newAccessToken = tokenService.GenerateAccessToken(principal.Claims, new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("SecretKey"))));
                var newRefreshToken = tokenService.GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                //userContext.SaveChanges();
                User.UpdateRefreshToken(user);
                return new ObjectResult(new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                });
            }
        }


        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            //var user = userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
            var ConnectionStringLocal = _configuration.GetValue<string>("ServidorLocal");
            using (IUser User = Factorizador.CrearConexionServicio(APIUsers.Library.Models.ConnectionType.MSSQL, ConnectionStringLocal))
            {
                APIUsers.Library.Models.User user = User.GetUser(username);
                if (user == null) return BadRequest();
                user.RefreshToken = null;
                //userContext.SaveChanges();
                User.UpdateRefreshToken(user);
                return NoContent();
            }
        }
    }
}
