using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SoMeSimulator.Data.Models
{
    /// <summary>
    /// Manages users and password
    /// </summary>
    public interface IUserService
    {

        /// <summary>
        /// Returns logged in Usr or null if not logged in.
        /// </summary>
        /// <returns></returns>
        Usr GetLoggedInUsr();    
        
        /// <summary>
        /// Check is user exists
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Usr VerifyUser(string user, string password);

        /// <summary>
        /// Hash password with salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string Hash(string password, string salt);

        /// <summary>
        /// Verifies that a password is corrent
        /// </summary>
        /// <param name="passwordHash"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool VerifyPassword(string passwordHash, string password);

        /// <summary>
        /// Creates hash from password and a random salt.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string NewHash(string password);
    }
}