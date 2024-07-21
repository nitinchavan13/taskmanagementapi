using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace TaskManagerApi.Controllers
{
    public class BaseController : ApiController
    {

        protected int GetUserId()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            return int.Parse(claims.First(i => i.Type == "userid").Value);
        }
    }
}
