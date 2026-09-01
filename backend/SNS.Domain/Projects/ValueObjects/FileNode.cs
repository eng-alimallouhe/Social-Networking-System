namespace SNS.Domain.Projects.ValueObjects;

public class FileNode
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = "file";

    public double? SizeKB { get; set; }
    public string? Url { get; set; }

    public List<FileNode> Children { get; set; } = new();
}