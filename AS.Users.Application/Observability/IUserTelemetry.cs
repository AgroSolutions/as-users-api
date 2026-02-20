namespace AS.Users.Application.Observability;

public interface IUserTelemetry
{
    void UserSignup(string? userId, string email, bool success, string? failureReason = null);  
    void UserLoginAttempt(string? userId, string email, bool success, string? failureReason = null);
    void CreatedUser(string userId, string email);
}
