namespace SNS.Application.Shared.Abstractions.Storage;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(
        Stream fileStream, 
        string contentType,
        string objectKey, 
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadFileStreamAsync(string objectKey, CancellationToken cancellationToken = default);

    string GetFilePublicUrl(string objectKey);

    Task<string> GetTemporaryUrlAsync(
        string objectKey,
        TimeSpan expires);

    Task DeleteFileAsync(string objectKey, CancellationToken cancellationToken = default);
}
