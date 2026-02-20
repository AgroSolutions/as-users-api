using AS.Users.Application.DTO;
using AS.Users.Domain.ValueObjects;
using AS.Users.Application.Services;
using AS.Users.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AS.Users.Application.Observability;

namespace AS.Users.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiBaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtService _jwtService;
        private readonly IUserTelemetry _userTelemetry;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            JwtService jwtService,
            IUserTelemetry userTelemetry)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _userTelemetry = userTelemetry;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new User(dto.Name, dto.Email);
            var password = new Password(dto.Password);

            var result = await _userManager.CreateAsync(user, password.PlainText);

            if (!result.Succeeded)
            {
                _userTelemetry.UserSignup(userId: null, email: dto.Email, success: false, failureReason: "identity_create_failed");
                return BadRequest(result.Errors);
            }

            _userTelemetry.UserSignup(userId: user.Id, email: dto.Email, success: true);

            await _userManager.AddToRoleAsync(user, "User");

            return CreatedResponse(user, "Usuário registrado com sucesso!");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    _userTelemetry.UserLoginAttempt(userId: null, email: dto.Email, success: false, failureReason: "user_not_found");
                    return UnauthorizedResponse("Usuário ou senha inválidos.");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
                if (!result.Succeeded)
                {
                    _userTelemetry.UserLoginAttempt(userId: user.Id, email: dto.Email, success: false, failureReason: "invalid_password");
                    return UnauthorizedResponse("Usuário ou senha inválidos.");
                }

                _userTelemetry.UserLoginAttempt(userId: user.Id, email: dto.Email, success: true);

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles);
                var response = new
                {
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.Email,
                        user.Name,
                        roles
                    }
                };

                return Success(response);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}