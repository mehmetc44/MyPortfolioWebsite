using AutoMapper;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.DTOs.AboutMe;
using Portfolio.Application.DTOs.File;
using Portfolio.Application.Repositories.AboutMe;
using Portfolio.Application.Repositories.SiteImageFile;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Infrastructure.Services;

public class AboutMeService : IAboutMeService
{
    private readonly IAboutMeWriteRepository _aboutMeWriteRepository;
    private readonly IAboutMeReadRepository _aboutMeReadRepository;
    private readonly ISiteImageFileWriteRepository _siteImageFileWriteRepository;
    private readonly ISiteImageFileReadRepository _siteImageFileReadRepository;
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public AboutMeService(
        IAboutMeWriteRepository aboutMeWriteRepository,
        IAboutMeReadRepository aboutMeReadRepository,
        ISiteImageFileWriteRepository siteImageFileWriteRepository,
        ISiteImageFileReadRepository siteImageFileReadRepository,
        IStorageService storageService,
        IMapper mapper)
    {
        _aboutMeWriteRepository = aboutMeWriteRepository;
        _aboutMeReadRepository = aboutMeReadRepository;
        _siteImageFileReadRepository = siteImageFileReadRepository;
        _siteImageFileWriteRepository = siteImageFileWriteRepository;
        _storageService = storageService;
        _mapper = mapper;
    }

    public async Task UpdateAboutMeAsync(UpdateAboutMeDto aboutMeDto, IFormFile? heroImage, IFormFile? profileImage)
    {
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

        // 2. Hero Image Yükleme İşlemi (Eğer dosya gönderilmişse)
        if (heroImage != null)
        {
            var heroDto = new SiteImageFileUploadDto
            {
                SiteImageType = SiteImageType.SiteHeroImage, // Enum değerini kendi projene göre ayarla
                Path = "aboutme-images", // Storage üzerinde kaydedileceği klasör/container adı
                FileName=heroImage.FileName
            };
            await UploadSiteImageAsync(heroDto, heroImage);
        }

        // 3. Profile Image Yükleme İşlemi (Eğer dosya gönderilmişse)
        if (profileImage != null && profileImage.Length > 0)
        {
            var profileDto = new SiteImageFileUploadDto
            {
                SiteImageType = SiteImageType.SiteProfileImage, // Enum değerini kendi projene göre ayarla
                Path = "aboutme-images",
                FileName=profileImage.FileName
            };
            await UploadSiteImageAsync(profileDto, profileImage);
        }

        await _aboutMeWriteRepository.SaveAsync();
        await _siteImageFileWriteRepository.SaveAsync();
    }
   public async Task UploadSiteImageAsync(SiteImageFileUploadDto dto, IFormFile file)
{
    if (file == null)
        throw new Exception("Yüklenecek fotoğraf bulunamadı veya dosya boş.");
    var existingSiteImage = await _siteImageFileReadRepository
        .GetSingleAsync(x => x.SiteImageType == dto.SiteImageType);

    string folderPath = Path.Combine("wwwroot", dto.Path);
    if (!Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath);
    if (existingSiteImage != null 
        && !string.IsNullOrEmpty(existingSiteImage.Path) 
        && !string.IsNullOrEmpty(existingSiteImage.FileName)
        && _storageService.HasFile(existingSiteImage.Path, existingSiteImage.FileName))
    {
        await _storageService.DeleteAsync(existingSiteImage.Path, existingSiteImage.FileName);
    }

    var files = new FormFileCollection(){file};
    var uploadResults = await _storageService.UploadAsync(dto.Path, files);
    var uploadedFileInfo = uploadResults.FirstOrDefault();
    if (existingSiteImage != null)
    {
        existingSiteImage.Path = uploadedFileInfo.pathOrContainerName;
        existingSiteImage.FileName = uploadedFileInfo.fileName;
        _siteImageFileWriteRepository.Update(existingSiteImage);
    }
    else
    {
        var newSiteImage = _mapper.Map<SiteImageFile>(dto);
        newSiteImage.Path = uploadedFileInfo.pathOrContainerName;
        newSiteImage.FileName = uploadedFileInfo.fileName;
        await _siteImageFileWriteRepository.AddAsync(newSiteImage);
    }
}
public Task<GetAboutMeDto> GetAboutMeAsync()
    {
        throw new NotImplementedException();
    }
}
