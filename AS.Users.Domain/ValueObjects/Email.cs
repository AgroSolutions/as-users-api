namespace AS.Users.Domain.ValueObjects;

public record Email
{
    public string? Address { get; }
    public Email(string value)
    {
        ValidateEmail(value);
        Address = value;
    }

    protected Email() { } // For EF Core

    private static void ValidateEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email não pode ser vazio.", nameof(value));
    }

    public override string ToString() => Address!;
}