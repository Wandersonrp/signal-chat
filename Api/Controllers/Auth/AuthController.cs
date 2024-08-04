using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using SignalChat.Api.Models;
using SignalChat.Api.Services.Users;
using System.Security.Claims;

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
            RedirectUri = Url.Action(nameof(LoginCallback))
        });
    }

    [HttpGet]
    public async Task<IActionResult> LoginCallback([FromServices] IUserService userService)
    {
        var result = await HttpContext
            .AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if(result is null)
        {
            return BadRequest();
        }

        var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

        if(email is null)
        {
            return BadRequest();
        }

        var user = await userService.GetUserByEmail(email);

        if(user is null)
        {
            await SaveUserDetails(result, userService);
        }

        return Redirect("https://localhost:7116");
    }  

    [HttpGet]
    [Route("signout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return Redirect("~/");
    }

    private async Task SaveUserDetails(AuthenticateResult result, IUserService userService)
    {
        var email = result?.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        var userName = result?.Principal?.FindFirst(ClaimTypes.Name)?.Value;
        var picture = User.Claims.Where(claim => claim.Type == "picture")?.FirstOrDefault()?.Value;

        var user = new UserModel
        { 
            Name = userName ?? string.Empty,
            Email = email ?? string.Empty,
            Picture = picture ?? string.Empty,
            DateJoined = DateTime.UtcNow
        };

        await userService.RegisterUser(user);
    }
}
