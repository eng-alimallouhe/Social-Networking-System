namespace SNS.Application.Shared.Contracts.Storage;

public sealed record UploadedFile(
    Stream Stream,
    string FileName,
    string ContentType,
    string Extension,
    long Length,
    FileType FileType = FileType.Generec
);


public enum FileType
{
    Generec,
    Image,
    Video,
    Folder,
    TextFile,
}