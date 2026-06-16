using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SNS.Application.Shared.Settings;
using SNS.Application.Shared.Abstractions.Storage;


namespace SNS.Infrastructure.Shared.Services.Storage;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioStorageSettings _settings;

    public MinioFileStorageService(IMinioClient minioClient, IOptions<MinioStorageSettings> settings)
    {
        _minioClient = minioClient;
        _settings = settings.Value;
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string contentType,
        string objectKey, 
        CancellationToken cancellationToken = default)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_settings.BucketName);
        bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if (!found)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_settings.BucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectKey)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        var protocol = _settings.UseSSL ? "https" : "http";
        return $"{protocol}://{_settings.Endpoint}/{_settings.BucketName}/{objectKey}";
    }

    public async Task<string> UploadStreamAsync(Stream stream, string objectKey, string contentType, CancellationToken cancellationToken = default)
    {
        return await UploadStreamInternalAsync(stream, objectKey, contentType, stream.Length, cancellationToken);
    }

    private async Task<string> UploadStreamInternalAsync(Stream stream, string objectKey, string contentType, long size, CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_settings.BucketName);
        bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if (!found)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_settings.BucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        // 2. ????? ??? ????? ???????? Builder Pattern
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectKey)
            .WithStreamData(stream)
            .WithObjectSize(size)
            .WithContentType(contentType);

        // 3. ???????
        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        // 4. ???? ?????? ?????? ???????
        var protocol = _settings.UseSSL ? "https" : "http";
        return $"{protocol}://{_settings.Endpoint}/{_settings.BucketName}/{objectKey}";
    }

    public async Task<Stream> DownloadFileStreamAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        // ?? MinIO? ??? ????? ???????? ??? Callback? ???? ?????? ??? MemoryStream
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectKey)
            .WithCallbackStream((stream) =>
            {
                stream.CopyTo(memoryStream);
            });

        await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task DeleteFileAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
    }
}
