using System;

namespace Portfolio.Application.Abstraction.Services;

using Microsoft.AspNetCore.Http;
using Portfolio.Application.DTOs.AboutMe;

public interface IAboutMeService
{
    Task<GetAboutMeDto> GetAboutMeAsync();
    Task UpdateAboutMeAsync(UpdateAboutMeDto aboutMeDto, IFormFile? heroImage, IFormFile? profileImage);
}
