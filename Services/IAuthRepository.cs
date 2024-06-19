using MyWebAPI.Models;

namespace MyWebAPI.Services
{
    public interface IAuthRepository
    {
        AuthResponseModel SignIn(LoginModel loginModel);
    }
}
