using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using AtonTask.DAL.Repositories;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;

namespace BLL
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;

        public AuthService(IConfiguration config, IUserRepository userRepo)
        {
            _config = config;
            _userRepo = userRepo;
        }

        public async Task<string> AuthenticateUser(string login, string password)
        {
            var user = await _userRepo.GetUserByLoginAsync(login);
            if (user.Password != password)
                throw new InvalidCredentialException();  
            return this.GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim("Login", user.Login),
                new Claim("IsAdmin", user.Admin ? "true" : "false"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}