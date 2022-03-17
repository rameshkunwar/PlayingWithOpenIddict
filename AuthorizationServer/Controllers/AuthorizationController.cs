using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace AuthorizationServer.Controllers
{
    public class AuthorizationController : Controller
    {

        //token endpoint
        [HttpPost("~/connect/token")]
        public IActionResult Exchange()
        {
            OpenIddictRequest? request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("OpenId connect request can't be retrieved");

            ClaimsPrincipal claims;

            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.

            if (request.IsClientCredentialsGrantType())
            {
                ClaimsIdentity? identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Subject (sub) is a required field, we use the client id as the subject identifier here.
                identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

                //adding claims
                identity.AddClaim("my-claim", "my-claim-value", OpenIddictConstants.Destinations.AccessToken);

                claims = new ClaimsPrincipal(identity);
                claims.SetScopes(request.GetScopes());
            }
            else
            {
                throw new InvalidOperationException("The specified grant type is not supported.");
            }

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        }
    }
}
