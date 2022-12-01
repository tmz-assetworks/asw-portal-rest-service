using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Helper
{
    public class TokenBase
    {
        public string? acces_token { get; set; }
        public string getObjectId()
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(acces_token);
            return jwtSecurityToken.Claims.First(claim => claim.Type == "oid").Value;
        }
        public string getRole()
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(acces_token);
            return jwtSecurityToken.Claims.First(claim => claim.Type == "roles").Value;
        }
    }
}
