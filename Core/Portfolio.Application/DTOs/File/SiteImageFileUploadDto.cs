using System;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.DTOs.File;

public class SiteImageFileUploadDto
{
    public string FileName {get; set;} = null!;
    public string Path {get; set;} = null!;
    public string? Storage {get;set;} 
    public SiteImageType SiteImageType {get;set;} 
}
