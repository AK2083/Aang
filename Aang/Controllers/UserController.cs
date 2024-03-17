using Aang.Model.DTO;
using Aang.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Aang.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepo.Login(model);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
                return BadRequest(new { message = "Username or Password is incorrect." });

            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            var isUserNameUnique = _userRepo.IsUniqueUser(model.Username);

            if (!isUserNameUnique)
                return BadRequest(new { message = $"User already exists." });

            var user = await _userRepo.Register(model);

            if (user == null) BadRequest(new { message = "Error" });

            return Ok(user);
        }
    }
}
