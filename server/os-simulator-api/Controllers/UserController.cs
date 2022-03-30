using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionGroups.Status;

namespace SoMeSimulator.Controllers
{
    [Route("api/[controller]")]
    public partial class UserController: ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("SignIn")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody]SignInPost signInPost)
        {
            var usr = _userService.VerifyUser(signInPost.Username, signInPost.Password);

            if (usr == null) return Unauthorized();

            await SignIn(usr);

            if (usr.ActiveSessionGroup != null && !(usr.ActiveSessionGroup.Status == SessionStatus.Cancelled || usr.ActiveSessionGroup.Status == SessionStatus.Finished))
            {
                return Ok(new { SessionGroupId = usr.ActiveSessionGroup.Id,
                    usr.ActiveSessionGroup.TypeableCode,
                    usr.ActiveSessionGroup.Scenario,
                    usr.UserRoles,
                    usr.ActiveSessionGroup.GroupName,
                    usr.ActiveSessionGroup.Status
                });
            }

            return Ok(new {UserRoles = usr.UserRoles});
        }

        private async Task SignIn(Usr usr)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString()),
                new Claim(ClaimTypes.Name, usr.Username.ToString()),
            };

            foreach (var role in usr.GetRoles())
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, 
                ClaimTypes.Name, 
                ClaimTypes.Role
                );
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }


        [Route("SignOut")]
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok("Signed Out");
        }

    }
}
