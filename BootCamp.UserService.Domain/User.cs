﻿namespace BootCamp.UserService.Domain;

public class User
{
    public required Guid Id { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }  
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
}
