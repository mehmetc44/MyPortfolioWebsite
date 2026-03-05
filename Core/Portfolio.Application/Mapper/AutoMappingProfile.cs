using System;
using AutoMapper;
using Portfolio.Application.DTOs.PersonalInfo;
using Portfolio.Application.DTOs.File;
using Portfolio.Domain.Entities;


namespace Portfolio.Application.Mapper;
public class AutoMappingProfile : Profile
{
    public AutoMappingProfile()
    {
        // DTO'dan Entity'e çeviri yapar. 
        // Mevcut bir entity'i güncelleyeceğimiz için bu map işlemi kullanılacak.
        CreateMap<UpdatePersonalInfoDto, PersonalInfo>();
        CreateMap<SiteImageFileUploadDto, SiteImageFile>();
        CreateMap<SiteImageFile, SiteImageFileUploadDto>();
    }
}