using System;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Enums;
using Portfolio.Infrastructure.Services;
using Portfolio.Infrastructure.Services.File;
using Portfolio.Infrastructure.Services.Storage;
using Portfolio.Infrastructure.Services.Storage.Azure;
using Portfolio.Infrastructure.Services.Storage.Local;

namespace Portfolio.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection service)
    {
        service.AddScoped<IStorageService, StorageService>();
        service.AddScoped<ISiteImageFileService, SiteImageFileService>();

    }
    public static void AddStorage<T>(this IServiceCollection service) where T : Storage, IStorage
    {
        service.AddScoped<IStorage, T>();
    }
    public static void AddStorage(this IServiceCollection service, StorageType storageType)
    {
        switch (storageType)
        {
            case StorageType.Local:
                service.AddScoped<IStorage, LocalStorage>();
                break;
            case StorageType.Azure:
                service.AddScoped<IStorage, AzureStorage>();
                break;
            default:
                service.AddScoped<IStorage, LocalStorage>();
                break;
        }
    }
}
