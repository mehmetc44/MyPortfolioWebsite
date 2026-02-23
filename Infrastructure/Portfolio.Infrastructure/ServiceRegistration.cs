using System;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Infrastructure.Enums;
using Portfolio.Infrastructure.Services;
using Portfolio.Infrastructure.Services.Storage;
using Portfolio.Infrastructure.Services.Storage.Azure;
using Portfolio.Infrastructure.Services.Storage.Local;

namespace Portfolio.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IStorageService, StorageService>();
        serviceCollection.AddScoped<IAboutMeService, AboutMeService>();
    }
    public static void AddStorage<T>(this IServiceCollection serviceCollection) where T : Storage, IStorage
    {
        serviceCollection.AddScoped<IStorage, T>();
    }
    public static void AddStorage(this IServiceCollection serviceCollection, StorageType storageType)
    {
        switch (storageType)
        {
            case StorageType.Local:
                serviceCollection.AddScoped<IStorage, LocalStorage>();
                break;
            case StorageType.Azure:
                serviceCollection.AddScoped<IStorage, AzureStorage>();
                break;
            default:
                serviceCollection.AddScoped<IStorage, LocalStorage>();
                break;
        }
    }
}
