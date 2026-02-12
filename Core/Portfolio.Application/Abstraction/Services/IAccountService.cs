using System;

namespace Portfolio.Application.Abstraction.Services;

public interface IAccountService
{
    Task<(bool isSuccess, string? errorMessage)> LoginAsync(string username, string password);
    Task LogoutAsync();
}
