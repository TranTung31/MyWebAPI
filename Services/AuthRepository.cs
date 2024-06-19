using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyWebAPI.Data;
using MyWebAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MyWebAPI.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MyDbContext _context;
        private readonly AppSettings _appsettings;
        public AuthRepository(MyDbContext context, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _context = context;
            _appsettings = optionsMonitor.CurrentValue;
        }

        private string GenerateToken(NguoiDung nguoiDung)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appsettings.SecretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, nguoiDung.HoTen),
                    new Claim(ClaimTypes.Email, nguoiDung.Email),
                    new Claim("UserName", nguoiDung.UserName),
                    new Claim("Id", nguoiDung.Id.ToString()),
                    new Claim("TokenId", Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }

        public AuthResponseModel SignIn(LoginModel loginModel)
        {
            var user = _context.NguoiDungs.SingleOrDefault(x => x.UserName == loginModel.UserName
                && x.Password == loginModel.Password);

            if (user == null)
            {
                return new AuthResponseModel
                {
                    IsSuccess = false,
                    Message = "Invalid username or password!"
                };
            }

            var token = GenerateToken(user);

            return new AuthResponseModel
            {
                IsSuccess = true,
                Message = "Sign in successfully!",
                Data = token.ToString()
            };
        }
    }
}
