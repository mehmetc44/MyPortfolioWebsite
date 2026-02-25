using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.DTOs.AboutMe;
using Portfolio.Application.Repositories.AboutMe;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Services;

public class AboutMeService : IAboutMeService
{
    private readonly IAboutMeWriteRepository _writeRepository;
    private readonly IAboutMeReadRepository _readRepository;
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public AboutMeService(
        IAboutMeWriteRepository writeRepository,
        IAboutMeReadRepository readRepository,
        IMapper mapper)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _mapper = mapper;
    }

    public async Task UpdateAboutMeAsync(UpdateAboutMeDto aboutMeDto)
    {
        var existingAboutMe = _readRepository.GetAll().FirstOrDefault();

        if (existingAboutMe == null)
        {
            var newAboutMe = _mapper.Map<AboutMe>(aboutMeDto);
            await _writeRepository.AddAsync(newAboutMe);
        }
        else
        {
            _mapper.Map(aboutMeDto, existingAboutMe);
            _writeRepository.Update(existingAboutMe);
        }
        await _writeRepository.SaveAsync();
    }

    public Task<GetAboutMeDto> GetAboutMeAsync()
    {
        throw new NotImplementedException();
    }

    public Task UploadHeroImageAsync(string PathOrContainer, IFormFile HeroImage)
    {
        throw new NotImplementedException();
    }

    public Task UploadProfileImageAsync(string PathOrContainer, IFormFile ProfileImage)
    {
        throw new NotImplementedException();
    }
}
