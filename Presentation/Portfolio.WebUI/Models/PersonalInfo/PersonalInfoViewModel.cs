using System;
using Azure.Storage.Blobs.Models;
using Microsoft.Net.Http.Headers;
using Portfolio.Application.DTOs.PersonalInfo;
using Portfolio.Domain.Entities;

namespace Portfolio.WebUI.Models.PersonalInfo;

public class PersonalInfoViewModel
{
    public PersonalInfoDto personalInfoDto = null!;
    public string? profilePhotoFullPath {get;set;}
    public string? heroPhotoFullPath {get;set;}
}
