namespace SNS.Application.Shared.Contracts.Storage;

/// <summary>
/// Represents an uploaded file model containing file stream, metadata, and classification type.
/// </summary>
/// <param name="Stream">The data stream of the uploaded file.</param>
/// <param name="FileName">The original file name including extension.</param>
/// <param name="ContentType">The MIME content type of the file.</param>
/// <param name="Extension">The file extension without the leading dot.</param>
/// <param name="Length">The total file size in bytes.</param>
/// <param name="FileType">The classified type of the file.</param>
public sealed record UploadedFile(
    Stream Stream,
    string FileName,
    string ContentType,
    string Extension,
    long Length,
    FileType FileType = FileType.Generec
);

/// <summary>
/// Specifies the category or classification type of an uploaded file.
/// </summary>
public enum FileType
{
    Generec,
    Image,
    Video,
    Folder,
    TextFile,
}