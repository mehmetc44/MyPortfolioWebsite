using System;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.DTOs.AboutMe;
using Portfolio.Application.Repositories.AboutMe;

namespace Portfolio.Infrastructure.Services;

public class AboutService : IAboutMeService
{
    IAboutMeWriteRepository _aboutMeWriteRepository;

    public AboutService(IAboutMeWriteRepository aboutMeWriteRepository)
    {
        _aboutMeWriteRepository = aboutMeWriteRepository;
    }

    public Task<GetAboutMeDto> GetAboutMeAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAboutMeAsync(UpdateAboutMeDto aboutMe)
    {
    
       throw new NotImplementedException();
    }
}
