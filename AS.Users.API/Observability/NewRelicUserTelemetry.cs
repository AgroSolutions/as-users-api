using AS.Users.Application.Observability;

namespace AS.Users.API.Observability;

public class NewRelicUserTelemetry : IUserTelemetry
{
        private const string ServiceName = "as.users.api";
        private const string Env = "dev";

    public void CreatedUser(string userId, string email) =>
        Record("AdminCreatedUser", userId, email, success: true, failureReason: null);

    public void UserLoginAttempt(string? userId, string email, bool success, string? failureReason = null) =>
        Record("UserLoginAttempt", userId, email, success, failureReason);

    public void UserSignup(string? userId, string email, bool success, string? failureReason = null) =>
        Record("UserSignup", userId, email, success, failureReason);

    private static void Record(string eventType, string? userId, string email, bool success, string? failureReason)
    {
        var emailDomain = email.Contains('@') ? email.Split('@').Last() : "";

        var attributes = new Dictionary<string, object>
        {
            ["service"] = ServiceName,
            ["env"] = Env,
            ["userId"] = userId ?? "",
            ["emailDomain"] = emailDomain,
            ["success"] = success ? 1 : 0,
            ["failureReason"] = failureReason ?? ""
        };

        NewRelic.Api.Agent.NewRelic.RecordCustomEvent(eventType, attributes);
    }

}
