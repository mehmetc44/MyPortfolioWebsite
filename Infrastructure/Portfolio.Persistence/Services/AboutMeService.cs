using AutoMapper;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.DTOs.AboutMe;
using Portfolio.Application.DTOs.File;
using Portfolio.Application.Repositories.AboutMe;
using Portfolio.Application.Repositories.SiteImageFile;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Persistence.Services;

public class AboutMeService : IAboutMeService
{
    private readonly IAboutMeWriteRepository _aboutMeWriteRepository;
    private readonly IAboutMeReadRepository _aboutMeReadRepository;
    private readonly ISiteImageFileService _siteImageFileService;
    private readonly IMapper _mapper;

    public AboutMeService(
        IAboutMeWriteRepository aboutMeWriteRepository,
        IAboutMeReadRepository aboutMeReadRepository,
        ISiteImageFileService siteImageFileService,
        IMapper mapper)
    {
        _aboutMeWriteRepository = aboutMeWriteRepository;
        _aboutMeReadRepository = aboutMeReadRepository;
        _siteImageFileService = siteImageFileService;
        _mapper = mapper;
    }

    public async Task UpdateAboutMeAsync(UpdateAboutMeDto aboutMeDto, IFormFile? heroImage, IFormFile? profileImage)
    {
        //AboutMe kaydını ekle veya güncelle
        var existingAboutMe = _aboutMeReadRepository.GetAll().FirstOrDefault();

        if (existingAboutMe == null)
        {
            var newAboutMe = _mapper.Map<AboutMe>(aboutMeDto);
            await _aboutMeWriteRepository.AddAsync(newAboutMe);
        }
        else
        {
            _mapper.Map(aboutMeDto, existingAboutMe);
            _aboutMeWriteRepository.Update(existingAboutMe);
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
        await _aboutMeWriteRepository.SaveAsync();
    }
    public Task<GetAboutMeDto> GetAboutMeAsync()
    {
        throw new NotImplementedException();
    }
}
