using AS.Users.Application.DTO;
using AS.Users.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AS.Users.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ApiBaseController
{
    private readonly IUserService _userService;

    public UserController(
        IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Success(users, "Lista de usuários retornada com sucesso.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Success(user, "Usuário encontrado.");
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserModel model)
    {
        var user = await _userService.CreateUserAsync(model);
        return CreatedResponse(user, "Usuário criado com sucesso.");
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PartialUpdateUser(string id, UpdateUserModel model)
    {
        await _userService.UpdateUserAsync(id, model);
        return Success("Usuário atualizado com sucesso.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}