using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using R2StorageApp.Interfaces;

namespace R2StorageApp.Services;

public class R2StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _config;

    public R2StorageService(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _config = config;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var bucketName = _config["CloudflareR2:BucketName"];

        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(putRequest);

        return $"{_config["CloudflareR2:PublicUrl"]}/{fileName}";
    }

    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            await _s3Client.DeleteObjectAsync(_config["CloudflareR2:BucketName"], fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[R2 WARNING] File ({fileName}) deletion error: {ex.Message}");
        }
    }
}
