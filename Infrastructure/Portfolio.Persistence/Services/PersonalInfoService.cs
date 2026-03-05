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

    public async Task UpdateAboutMeAsync(UpdatePersonalInfoDto aboutMeDto, IFormFile? heroImage, IFormFile? profileImage)
    {
        //AboutMe kaydını ekle veya güncelle
        var existingAboutMe = _personalInfoReadRepository.GetAll().FirstOrDefault();

        if (existingAboutMe == null)
        {
            var newAboutMe = _mapper.Map<PersonalInfo>(aboutMeDto);
            await _personalInfoWriteRepository.AddAsync(newAboutMe);
        }
        else
        {
            _mapper.Map(aboutMeDto, existingAboutMe);
            _personalInfoWriteRepository.Update(existingAboutMe);
        }

        //Hero Image yükleme
        if (heroImage != null && heroImage.Length > 0)
        {
            var heroDto = new SiteImageFileUploadDto
            {
                SiteImageType = SiteImageType.SiteHeroImage,
                Path = "aboutme-images"
            };

            await _siteImageFileService.UploadAsync(heroDto, heroImage);
        }

        //Profile Image yükleme
        if (profileImage != null && profileImage.Length > 0)
        {
            var profileDto = new SiteImageFileUploadDto
            {
                SiteImageType = SiteImageType.SiteProfileImage,
                Path = "aboutme-images"
            };
            await _siteImageFileService.UploadAsync(profileDto, profileImage);
        }
        await _personalInfoWriteRepository.SaveAsync();
    }
    public Task<PersonalInfoDto> GetAboutMeAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddSkillAsync()
    {
        throw new NotImplementedException();
    }
}
