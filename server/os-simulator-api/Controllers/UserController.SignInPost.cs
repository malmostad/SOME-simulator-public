using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace SoMeSimulator.Controllers
{
    public partial class UserController
    {
        public class SignInPost
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

    }
}