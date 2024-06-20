using MyWebAPI.Models;

namespace MyWebAPI.Services
{
    public interface IAuthRepository
    {
        ApiResponseModel SignIn(LoginModel loginModel);
        ApiResponseModel RefreshToken(TokenModel tokenModel);
    }
}
