using System;
using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities;

public class SiteImageFile : File
{
    public SiteImageType SiteImageType {get;set;} 
}
