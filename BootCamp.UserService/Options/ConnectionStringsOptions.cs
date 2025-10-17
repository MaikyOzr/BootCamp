namespace BootCamp.UserService.Options;

public sealed class ConnectionStringsOptions
{
    public const string SectionName = "ConnectionStrings";

    
    public required string DefaultConnection { get; init; } = string.Empty;
}
