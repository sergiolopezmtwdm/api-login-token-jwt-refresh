using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_TAREA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        [HttpGet, Authorize(Roles = "Manager")]
        //[HttpGet, Authorize(Roles = "Operador")]
        ////[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "Ramón Atilano", "Maricela Méndez" };
        //}
        //[HttpGet, Authorize]
        public IEnumerable<string> Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{
            //    IEnumerable<Claim> claims = identity.Claims;
            //    // or
            //    //identity.FindFirst("ClaimName").Value;

            //}

            IEnumerable<Claim> claim = identity.Claims;
            var listaRoles = claim                
                .Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value)
                .ToList();
            return new string[] { "John Doe", "Jane Doe" };
        }
    }
}
