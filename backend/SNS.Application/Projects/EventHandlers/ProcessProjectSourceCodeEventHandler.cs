using MediatR;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.Events;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Projects.Events;
using SNS.Domain.Projects.ValueObjects;
using SNS.Domain.Shared.Abstractions.Repositories;
using System.IO.Compression;
using System.Text.Json;

namespace SNS.Application.Projects.EventHandlers;

public class ProcessProjectSourceCodeEventHandler : INotificationHandler<DomainEventNotification<ProjectSourceCodeUploadedEvent>>
{
    private readonly IFileStorageService _storageService;
    private readonly ISoftDeletableRepository<Project> _projectRepository;

    public ProcessProjectSourceCodeEventHandler(
        IFileStorageService storageService,
        ISoftDeletableRepository<Project> projectRepository)
    {
        _storageService = storageService;
        _projectRepository = projectRepository;
    }

    public async Task Handle(DomainEventNotification<ProjectSourceCodeUploadedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        using var zipStream = await _storageService.DownloadFileStreamAsync(domainEvent.TempZipObjectKey, cancellationToken);
        
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

        var rootNode = new FileNode { Name = "root", Type = "folder" };
        
        var ignoredPaths = new[] { "node_modules/", "bin/", "obj/", ".git/", ".vs/" };

        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name) || entry.FullName.Contains("__MACOSX")) continue;
            if (ignoredPaths.Any(ignored => entry.FullName.Contains(ignored, StringComparison.OrdinalIgnoreCase))) continue;

            using var entryStream = entry.Open();
            string finalKey = $"projects/{domainEvent.ProjectId}/source-code/{entry.FullName}";

            string fileUrl = await _storageService.UploadFileAsync(
                entryStream,
                finalKey,
                "application/octet-stream",
                cancellationToken);

            double sizeKb = Math.Round((double)entry.Length / 1024, 2);
            AddPathToTree(rootNode, entry.FullName, sizeKb, fileUrl);
        }

        string jsonTree = JsonSerializer.Serialize(rootNode.Children, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var project = await _projectRepository.GetByIdAsync(domainEvent.ProjectId, cancellationToken);

        if (project != null)
        {
            project.MarkSourceCodeAsReady(jsonTree);
        }

        await _storageService.DeleteFileAsync(domainEvent.TempZipObjectKey, cancellationToken);
    }

    private void AddPathToTree(FileNode root, string fullPath, double sizeKb, string url)
    {
        var parts = fullPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var currentNode = root;

        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            var isFile = (i == parts.Length - 1);
            var existingChild = currentNode.Children.FirstOrDefault(c => c.Name == part);

            if (existingChild == null)
            {
                existingChild = new FileNode
                {
                    Name = part,
                    Type = isFile ? "file" : "folder",
                    SizeKB = isFile ? sizeKb : null,
                    Url = isFile ? url : null
                };
                currentNode.Children.Add(existingChild);
            }
            currentNode = existingChild;
        }
    }
}
