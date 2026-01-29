using Microsoft.AspNetCore.Http;

namespace AS.Users.Application.Exceptions;

public class NotFoundException : BaseCustomException
{
    public NotFoundException(string message = "Recurso não encontrado.")
        : base(StatusCodes.Status404NotFound, message) { }
}