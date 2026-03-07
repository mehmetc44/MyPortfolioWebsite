using System;

namespace Portfolio.Application.Abstraction.Services;

using Microsoft.AspNetCore.Http;
using Portfolio.Application.DTOs.PersonalInfo;
using Portfolio.Domain.Entities;

public interface IPersonalInfoService
{
    Task<PersonalInfoDto> GetPersonalInfoAsync();
    Task UpdatePersonalInfoAsync(UpdatePersonalInfoDto aboutMeDto);
}
