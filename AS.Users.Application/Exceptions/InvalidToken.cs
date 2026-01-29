namespace AS.Users.Application.Exceptions;
public class InvalidTokenException : BusinessErrorDetailsException
{
    public InvalidTokenException(string code, string message) : base(message)
    {
    }
}