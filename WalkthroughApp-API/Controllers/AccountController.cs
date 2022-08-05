using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;
using WalkthroughApp_API.Helpers;

namespace WalkthroughApp_API.Controllers
{
    public class AccountController : WalkthroughAppApiController
    {
        private readonly DataContext _context;
        private readonly IEncodePassword _encodePassword;

        public AccountController(DataContext context, IEncodePassword encodePassword)
        {
            _context = context;
            _encodePassword = encodePassword;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration newUser)
        {
            if (IsUserNameValid(newUser))
                return StatusCode((int)HttpStatusCode.BadRequest, "Username is invalid.");

            if (IsPasswordValid(newUser))
                return StatusCode((int)HttpStatusCode.BadRequest, "Password is invalid.");

            if (IsJobTitleValid(newUser))
                return StatusCode((int)HttpStatusCode.BadRequest, "Job Title is invalid.");

            if (DoesUserExist(newUser.UserName).Result)
                return StatusCode((int)HttpStatusCode.Conflict, "Username already exists.");


            var encodePassword = _encodePassword.EncodePassword(newUser.Password);

            var user = new User
            {
                UserName = newUser.UserName.ToLower(),
                PasswordHash = encodePassword.Item1,
                PasswordSalt = encodePassword.Item2,
                JobTitle = newUser.JobTitle
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDetails loginDetails)
        {
            if (!DoesUserExist(loginDetails.Username).Result) return NotFound("Invalid user");

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName.ToLower().Equals(loginDetails.Username.ToLower()));

            var encodedPassword = _encodePassword.EncodePasswordWithKey(user.PasswordSalt, loginDetails.Password);

            if (DoesPasswordMatch(encodedPassword, user))
            {
                return Unauthorized("Invalid password");
            }

            return Ok(user);
        }

        private static bool DoesPasswordMatch(byte[] encodedPassword, User user) => 
            encodedPassword.Where((t, i) => t != user.PasswordHash[i]).Any() || encodedPassword.Length == 0;

        private static bool IsJobTitleValid(UserRegistration newUser) => 
            string.IsNullOrWhiteSpace(newUser.JobTitle);

        private static bool IsPasswordValid(UserRegistration newUser) => 
            string.IsNullOrWhiteSpace(newUser.Password);

        private static bool IsUserNameValid(UserRegistration newUser) => 
            string.IsNullOrWhiteSpace(newUser.UserName);

        private async Task<bool> DoesUserExist(string username) => 
            await _context.Users.AnyAsync(p => p.UserName == username.ToLower());
    }
}
