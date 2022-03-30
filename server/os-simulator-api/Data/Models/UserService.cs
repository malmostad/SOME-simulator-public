using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.EntityFrameworkCore;
using SoMeSimulator.Data.Models.Defaults;
using SoMeSimulator.Helpers;

namespace SoMeSimulator.Data.Models
{
    public class UserService: IUserService
    {
        private readonly SoMeContext _dbContext;
        private readonly HttpContext _httpContext;
        private HashAlgorithm _hashAlgorithm;

        public UserService(SoMeContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Usr GetLoggedInUsr()
        {
            try
            {
                var claimsIdentity = _httpContext.User.Identity as ClaimsIdentity;

                var usrIdStr = claimsIdentity?.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

                var usrId = Int32.Parse(usrIdStr);
                return _dbContext.Users.FindById(usrId);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Verifies a user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Usr VerifyUser(string username, string password)
        {
            if (_dbContext.Users != null)
            {
                var usr = _dbContext.Users.Include(u => u.UserRoles).SingleOrDefault(u => u.Username == username);

                if (usr == null) return null;

                return VerifyPassword(usr.Password, password) ? usr: null;
            }

            return null;
        }
        /// <summary>
        /// Verify passwordhash and password
        /// </summary>
        /// <param name="passwordHash"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool VerifyPassword(string passwordHash, string password)
        {
            return passwordHash == Hash(password, passwordHash.Substring(0, 10));
        }

        /// <summary>
        /// Creates hash from password and a random salt.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string NewHash(string password)
        {
            var salt = AlphaNumericCode.Generate(
                DefaultValues.SaltLength
            ).ToLower();

            return Hash(password, salt);
        }
        
        /// <summary>
        /// Calculates hash from a specific password and hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string Hash(string password, string salt)
        {
            _hashAlgorithm = SHA256.Create();

            var passwordHashBytes = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(salt + password));

            for (var i = 0; i < DefaultValues.HashRounds; i++)
            {
                passwordHashBytes = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
            }

            string hash = string.Empty;

            foreach (byte passwordByte in passwordHashBytes)
            {
                hash += passwordByte.ToString("x2");
            }

            return salt + hash;
        }
    }
}