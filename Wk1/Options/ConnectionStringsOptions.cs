namespace Wk1.Options;

public class ConnectionStringsOptions
{
    public const string SectionName = "ConnectionStrings";

    
    public required string DefaultConnection { get; init; } = string.Empty;
}
