using Aang.Database;
using Aang.Model;
using Aang.Model.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aang.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _db;
        private string _secretKey;

        public UserRepository(ApplicationDBContext db, IConfiguration config)
        {
            _db = db;
            _secretKey = config.GetValue<string>("ApiSettings:SecretKey") ?? "Bananerama";
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.User.FirstOrDefault(x => x.Username == username);

            return user == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {
            var user = _db.User.FirstOrDefault(usr => usr.Username == loginRequest.Username &&
                usr.Password == loginRequest.Password);

            if (user == null) return new LoginResponseDTO()
            {
                Token = string.Empty,
                User = user,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.ID.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var response = new LoginResponseDTO()
            {
                User = user,
                Token = tokenHandler.WriteToken(token)
            };

            return response;
        }

        public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequest)
        {
            var user = new LocalUser()
            {
                Username = registrationRequest.Username,
                Password = registrationRequest.Password,
                Name = registrationRequest.Name,
                Role = registrationRequest.Role,
            };

            _db.User.Add(user);
            await _db.SaveChangesAsync();
            user.Password = string.Empty;

            return user;
        }
    }
}
