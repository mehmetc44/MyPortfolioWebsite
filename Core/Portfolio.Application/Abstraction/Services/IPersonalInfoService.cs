using System;

namespace Portfolio.Application.Abstraction.Services;

using Microsoft.AspNetCore.Http;
using Portfolio.Application.DTOs.PersonalInfo;

public interface IPersonalInfoService
{
    Task<PersonalInfoDto> GetAboutMeAsync();
    Task UpdateAboutMeAsync(UpdatePersonalInfoDto aboutMeDto, IFormFile? heroImage, IFormFile? profileImage);
    Task AddSkillAsync();
}
