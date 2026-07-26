using SNS.Application.Shared.Contracts.Storage;

namespace SNS.API.Extensions;

public static class FormFileExtensions
{
    public static UploadedFile ToUploadedFile(this IFormFile file)
    {
        return new UploadedFile(
            Stream: file.OpenReadStream(),
            FileName: file.FileName,
            ContentType: file.ContentType,
            Extension: Path.GetExtension(file.FileName)
                .TrimStart('.')
                .ToLowerInvariant(),
            Length: file.Length
        );
    }
}