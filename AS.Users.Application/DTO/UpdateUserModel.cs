using System.ComponentModel.DataAnnotations;

namespace AS.Users.Application.DTO;

public class UpdateUserModel
{
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "O email fornecido não é válido.")]
    public string? Email { get; set; }
}