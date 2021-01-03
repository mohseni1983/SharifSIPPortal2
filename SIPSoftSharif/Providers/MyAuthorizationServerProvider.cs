using Microsoft.Owin.Security.OAuth;
using SIPSoftSharif.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Providers.Entities;

namespace SIPSoftSharif.Providers
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            using(UserMasterRepository _repo=new UserMasterRepository())
            {
                userModel user = _repo.ValidateUser(context.UserName, context.Password);
                    if (user == null)
                    {
                        context.SetError("invalid_grant", "نام کاربری و رمز عبور اشتباه است");
                        return;
                    }
                    var idnetity = new ClaimsIdentity(context.Options.AuthenticationType);
                    if (user.MadadkarId == 0)
                        idnetity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                        
                    else
                    {
                        idnetity.AddClaim(new Claim(ClaimTypes.Role, "Madadkar"));
                        idnetity.AddClaim(new Claim("MadadkarId", user.MadadkarId.ToString()));

                    }
                    idnetity.AddClaim(new Claim(ClaimTypes.Name, user.Firstname + " " + user.Lastname));
                    idnetity.AddClaim(new Claim("UserId", user.UserId.ToString()));
                context.Validated(idnetity);

                }
            }
          
       

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
    }
}