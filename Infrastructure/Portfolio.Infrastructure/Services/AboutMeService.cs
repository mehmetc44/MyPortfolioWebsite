using System;
using AutoMapper;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.DTOs.AboutMe;
using Portfolio.Application.Repositories.AboutMe;

namespace Portfolio.Infrastructure.Services;

public class AboutMeService : IAboutMeService
{
    private readonly IAboutMeWriteRepository _writeRepository;
    private readonly IAboutMeReadRepository _readRepository; // Veriyi çekmek için gerekli
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
        // 1. Veritabanındaki mevcut kaydı getir (Veritabanında tek bir "Hakkımda" kaydı olduğunu varsayıyoruz)
        // Not: Kendi ReadRepository'nizdeki uygun metoda göre (örneğin GetAll().FirstOrDefault() vb.) uyarlayın.
        var existingAboutMe =  _readRepository.GetAll().FirstOrDefault(); 

        if (existingAboutMe == null)
        {
            throw new Exception("Güncellenecek Hakkımda (AboutMe) kaydı bulunamadı.");
        }

        // 2. Mapping: DTO'dan gelen yeni verileri, veritabanından çektiğimiz mevcut entity'nin üzerine yaz
        _mapper.Map(aboutMeDto, existingAboutMe);

        // 3. Repository'deki Update metodunu çağır (Tracking yapılıyorsa buna gerek bile kalmayabilir ama pattern gereği ekliyoruz)
        _writeRepository.Update(existingAboutMe);

        // 4. Değişiklikleri veritabanına kaydet
        await _writeRepository.SaveAsync();
    }

    public Task<GetAboutMeDto> GetAboutMeAsync()
    {
        throw new NotImplementedException();
    }

}
