using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyWebAPI.Data;
using MyWebAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        // Hàm tạo mới AccessToken và RefreshToken, lưu RefreshToken vào trong database
        private TokenModel GenerateToken(NguoiDung nguoiDung)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appsettings.SecretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, nguoiDung.HoTen),
                    new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserName", nguoiDung.UserName),
                    new Claim("Id", nguoiDung.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            var accessToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            // Lưu RefreshToken vào database
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = nguoiDung.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                JwtId = token.Id,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            _context.SaveChanges();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public string GenerateRefreshToken()
        {
            var stringRandom = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(stringRandom);
                return Convert.ToBase64String(stringRandom);
            }
        }

        public ApiResponseModel SignIn(LoginModel loginModel)
        {
            var user = _context.NguoiDungs.SingleOrDefault(x => x.UserName == loginModel.UserName
                && x.Password == loginModel.Password);

            if (user == null)
            {
                return new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "Invalid username or password!"
                };
            }

            var dataToken = GenerateToken(user);

            return new ApiResponseModel
            {
                IsSuccess = true,
                Message = "Sign in successfully!",
                Data = dataToken
            };
        }

        public ApiResponseModel RefreshToken(TokenModel tokenModel)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appsettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                // Tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                // Ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false, // Không kiểm tra token hết hạn
            };


            // Trường hợp 1: Check token xem có đúng định dạng không
            var tokenInVerify = jwtTokenHandler.ValidateToken(tokenModel.AccessToken, validationParameters,
                out var validatedToken);

            // Trường hợp 2: Check thuật toán
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals
                    (SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                if (!result)
                {
                    return new ApiResponseModel
                    {
                        IsSuccess = false,
                        Message = "Invalid access token!"
                    };
                }
            }

            // Trường hợp 3: Check accesstoken đã hết hạn chưa
            var utcExpireDate = long.Parse(tokenInVerify.Claims.FirstOrDefault(x =>
            x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);

            if (expireDate > DateTime.UtcNow)
            {
                return new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "Access token has not yet expired!"
                };
            }

            // Trường hợp 4: Check refresh token có trong database hay không
            var storedRefreshToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == tokenModel.RefreshToken);
            if (storedRefreshToken == null)
            {
                return new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "Refresh token does not exist!"
                };
            }

            // Trường hợp 5: Check refresh token đã được used hoặc revoked
            if (storedRefreshToken.IsUsed)
            {
                return new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "Refresh token has been used!"
                };
            }

            if (storedRefreshToken.IsRevoked)
            {
                return new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "Refresh token has been revoked!"
                };
            }

            // Trường hợp 6: Check accessToken Id có bằng JwtId trong refreshToken
            var jti = tokenInVerify.Claims.FirstOrDefault(x => x.Type ==
            JwtRegisteredClaimNames.Jti).Value;

            if (storedRefreshToken.JwtId != jti)
            {
                return new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "Token doesn't match!"
                };
            }

            // Cập nhật refreshToken
            storedRefreshToken.IsUsed = true;
            storedRefreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            _context.SaveChanges();

            // Tạo mới token
            var user = _context.NguoiDungs.SingleOrDefault(x => x.Id == storedRefreshToken.UserId);
            var token = GenerateToken(user);

            return new ApiResponseModel
            {
                IsSuccess = true,
                Message = "Refresh token successfully!",
                Data = token
            };
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
            return dateTimeInterval;
        }
    }
}
