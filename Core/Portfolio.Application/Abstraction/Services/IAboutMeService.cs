using System;

namespace Portfolio.Application.Abstraction.Services;

using Portfolio.Application.DTOs.AboutMe;
using Portfolio.Domain.Entities;

public interface IAboutMeService
{
    Task<GetAboutMeDto> GetAboutMeAsync();
    Task UpdateAboutMeAsync(UpdateAboutMeDto aboutMe);
}
