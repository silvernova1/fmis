using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fmis.Areas.Identity.Data
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<fmisUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<fmisUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(fmisUser user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);

            Console.WriteLine("waterss");
            Console.WriteLine(user.UserName);
            Console.WriteLine(user.Year);
            Console.WriteLine(user.YearId);
            /*if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                claimsIdentity.AddClaims(new[] {
                    new Claim("YearlyRef", user.Year),
                    new Claim("YearlyRefId", user.YearId.ToString())
                });
            }*/
            if (claimsIdentity.FindFirst("YearlyRef") is null)
            {
                var yearRef = new Claim("YearlyRef", user.Year);
                claimsIdentity.AddClaim(yearRef);
            }
            if (claimsIdentity.FindFirst("YearlyRefId") is null)
            {
                var yearRefId = new Claim("YearlyRefId", user.YearId.ToString());
                claimsIdentity.AddClaim(yearRefId);
            }

            // You can add more properties that you want to expose on the User object below

            return claimsIdentity;
        }
    }
}