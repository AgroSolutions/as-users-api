using Microsoft.AspNetCore.Http;

namespace AS.Users.Application.Exceptions;

public class BusinessErrorDetailsException : BaseCustomException
{
    public BusinessErrorDetailsException(string message)
        : base(StatusCodes.Status400BadRequest, message) { }
}