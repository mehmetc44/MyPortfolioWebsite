using AutoMapper;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.DTOs.PersonalInfo;
using Portfolio.Application.DTOs.File;
using Portfolio.Application.Repositories.PersonalInfo;
using Portfolio.Application.Repositories.SiteImageFile;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Persistence.Services;

public class PersonalInfoService : IPersonalInfoService
{
    private readonly IPersonalInfoWriteRepository _personalInfoWriteRepository;
    private readonly IPersonalInfoReadRepository _personalInfoReadRepository;
    private readonly ISiteImageFileService _siteImageFileService;
    private readonly IMapper _mapper;

    public PersonalInfoService(
        IPersonalInfoWriteRepository personalInfoWriteRepository,
        IPersonalInfoReadRepository personalInfoReadRepository,
        ISiteImageFileService siteImageFileService,
        IMapper mapper)
    {
        _personalInfoWriteRepository = personalInfoWriteRepository;
        _personalInfoReadRepository = personalInfoReadRepository;
        _siteImageFileService = siteImageFileService;
        _mapper = mapper;
    }

    public async Task UpdatePersonalInfoAsync(UpdatePersonalInfoDto updatePersonalInfoDto)
    {
        var existingPersonalInfo = _personalInfoReadRepository.GetAll().FirstOrDefault();

        if (existingPersonalInfo == null)
        {
            var newPersonalInfo = _mapper.Map<PersonalInfo>(updatePersonalInfoDto);
            await _personalInfoWriteRepository.AddAsync(newPersonalInfo);
        }
        else
        {
            _mapper.Map(updatePersonalInfoDto, existingPersonalInfo);
            _personalInfoWriteRepository.Update(existingPersonalInfo);
        }
        await _personalInfoWriteRepository.SaveAsync();
    }

    public Task<PersonalInfoDto> GetPersonalInfoAsync()
    {
        var existingPersonalInfo = _personalInfoReadRepository.GetAll().FirstOrDefault();
        if (existingPersonalInfo == null)
        {
            throw new Exception("Kişisel bilgiler için veri bulunamadı.");
        }
        var personalInfoDto = _mapper.Map<PersonalInfoDto>(existingPersonalInfo);
        return Task.FromResult(personalInfoDto);
    }

}
