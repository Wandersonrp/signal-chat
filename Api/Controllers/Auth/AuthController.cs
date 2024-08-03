using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace SignalChat.Api.Controllers.Auth;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet]
    [Route("google-login")]
    public async Task GoogleLoginAsync()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = "/"
        });
    }

    [HttpGet]
    [Route("signout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return Redirect("~/");
    }
}
