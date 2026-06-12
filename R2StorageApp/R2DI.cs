using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R2StorageApp.Interfaces;
using R2StorageApp.Services;

namespace R2StorageApp;

public static class R2DI
{
    public static IServiceCollection AddR2StorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        var r2Config = configuration.GetSection("CloudflareR2");

        var s3Config = new AmazonS3Config
        {
            ServiceURL = r2Config["ServiceUrl"],
            ForcePathStyle = true
        };

        services.AddSingleton<IAmazonS3>(_ =>
        {
            return new AmazonS3Client(
                r2Config["AccessKey"],
                r2Config["SecretKey"],
                s3Config
            );
        });

        services.AddScoped<IStorageService, R2StorageService>();

        return services;
    }
}