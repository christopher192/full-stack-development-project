using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Data.Models;
using NuGet.Common;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtHandler _jwtHandler;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, JwtHandler jwtHandler)
        {
            _context = context;
            _userManager = userManager;
            _jwtHandler = jwtHandler;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password)) 
            {
                return Unauthorized(new LoginResult()
                {
                    Success = false,
                    Message = "Invalid Email or Password."
                });
            }
  
            var secToken = await _jwtHandler.GetTokenAsync(user);
            var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);

            return Ok(new LoginResult()
            {
                Success = true,
                Message = "Login successful",
                Token = jwt,
                Expiration = secToken.ValidTo
            });
        }


        [HttpGet("Me")]
        public IActionResult Me()
        {
            var claimsPrincipal = User;

            if (claimsPrincipal.Identity != null) {
                var identity = (ClaimsIdentity)claimsPrincipal.Identity;
                var claim = identity.FindFirst(ClaimTypes.Name);

                if (claim != null)
                {
                    var user = _context.Users.Where(a => a.Email == claim.Value).FirstOrDefault();

                    if (user != null)
                    {

                        return Ok(user);
                    }
                }
            }

            return Ok();
        }
    }
}
