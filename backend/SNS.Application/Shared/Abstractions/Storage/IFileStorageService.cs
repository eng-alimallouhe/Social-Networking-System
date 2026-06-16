namespace SNS.Application.Shared.Abstractions.Storage;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(
        Stream fileStream, 
        string contentType,
        string objectKey, 
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadFileStreamAsync(string objectKey, CancellationToken cancellationToken = default);

    Task DeleteFileAsync(string objectKey, CancellationToken cancellationToken = default);
}
