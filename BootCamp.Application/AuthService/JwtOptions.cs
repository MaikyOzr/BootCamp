﻿namespace BootCamp.Application.AuthService;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
}
