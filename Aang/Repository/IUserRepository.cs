using Aang.Model;
using Aang.Model.DTO;

namespace Aang.Repository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequest);
    }
}
