using CSharp_Challenge.Dtos;
using CSharp_Core.Repository;
using CSharp_Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSharp_Challenge.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IRepository _repository;
        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, IRepository repository)
        {
            _userManager = userManager;
            _config = config;
            _repository = repository;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var Password = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!Password)
            {
                return BadRequest("Invalid credentials");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var UserRoles = roles.ToArray();

            var token = _repository.GenerateToken(model.Email, model.Password, _config, UserRoles);

            return Ok(token);
        }
    }
}

