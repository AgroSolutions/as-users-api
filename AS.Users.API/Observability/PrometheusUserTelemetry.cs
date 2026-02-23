using AS.Users.Application.Observability;
using Prometheus;

namespace AS.Users.API.Observability;

public class PrometheusUserTelemetry : IUserTelemetry
{
    private static readonly Counter CreatedUserTotal = Metrics.CreateCounter(
        "as_users_created_user_total", "Usuários criados",
        new CounterConfiguration { LabelNames = new[] { "service", "env", "email_domain" } });

    private static readonly Counter UserSignupTotal = Metrics.CreateCounter(
        "as_users_signup_total", "Tentativas de signup",
        new CounterConfiguration { LabelNames = new[] { "service", "env", "result", "failure_reason", "email_domain" } });

    private static readonly Counter UserLoginAttemptTotal = Metrics.CreateCounter(
        "as_users_login_attempt_total", "Tentativas de login",
        new CounterConfiguration { LabelNames = new[] { "service", "env", "result", "failure_reason", "email_domain" } });

    private const string ServiceName = "as.users.api";
    private const string Env = "dev";

    public void CreatedUser(string userId, string email)
    {
        var emailDomain = GetEmailDomain(email);
        CreatedUserTotal.WithLabels(ServiceName, Env, emailDomain).Inc();
    }

    public void UserLoginAttempt(string? userId, string email, bool success, string? failureReason = null)
    {
        var domain = GetEmailDomain(email);
        UserLoginAttemptTotal
            .WithLabels(ServiceName, Env, success ? "success" : "failure",
                       failureReason ?? "unknown", domain)
            .Inc();
    }

    public void UserSignup(string? userId, string email, bool success, string? failureReason = null)
    {
        var domain = GetEmailDomain(email);
        UserSignupTotal
            .WithLabels(ServiceName, Env, success ? "success" : "failure",
                       failureReason ?? "unknown", domain)
            .Inc();
    }

    private static string GetEmailDomain(string email)
        => email.Contains('@') ? email.Split('@').Last() : "";
}
