using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BoxSample.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        //[HttpGet]
        //public IActionResult Login(string returnUrl = "/")
        //{
        //    return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, new string[] { "github" });
        //}

        [HttpGet]
        public IActionResult LoginBox()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/auth/isValidUser" }, new string[] { "box" });
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            //This removes the cookie assigned to the user login.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> IsValidUser()
        {
            //Check email
            var email = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email) || !email.EndsWith("@organo.co.jp"))
            {
                //If email is incorrect, force logout
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/invalidUser-view");
            }
            else
                return LocalRedirect("/");
        }
    }
}
