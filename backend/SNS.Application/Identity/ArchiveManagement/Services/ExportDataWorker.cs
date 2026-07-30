using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SNS.Application.Identity.ArchiveManagement.Services;


public sealed class ExportDataWorker : IExportDataWorker
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ExportDataRequest> _exportDataRequestRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;

    public ExportDataWorker(
        IApplicationDbContext dbContext,
        IRepository<ExportDataRequest> exportDataRequestRepo,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IEmailSenderService emailSenderService,
        IEmailTemplateProvider emailTemplateProvider) 
    {
        _dbContext = dbContext;
        _exportDataRequestRepo = exportDataRequestRepo;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _emailSenderService = emailSenderService;
        _emailTemplateProvider = emailTemplateProvider;
    }

    public async Task ProcessExportAsync(Guid requestId)
    {
        var exportRequest = await _exportDataRequestRepo.GetByIdAsync(requestId);

        if (exportRequest == null) return;

        string jsonPath = string.Empty;
        string zipPath = string.Empty;

        try
        {
            exportRequest.MoveToProcessing();
            await _unitOfWork.CompleteAsync(CancellationToken.None);

            var userId = exportRequest.UserId;

            var userData = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    AccountInformation = new
                    {
                        u.Id,
                        u.UserProfile.FullName,
                        u.UserName,
                        u.Email,
                        u.CreatedAt,
                        u.PreferredLanguage
                    },
                    IdentityChangeHistory = _dbContext.IdentityArchives
                        .Where(ia => ia.UserId == userId)
                        .Select(ia => new { ia.OldUserIdentifier, ia.NewUserIdentifier, ia.Type, ia.CreatedAt })
                        .ToList(),
                    PasswordChangeHistory = _dbContext.PasswordArchives
                        .Where(pa => pa.UserId == userId)
                        .Select(pa => new { pa.CreatedAt })
                        .ToList(),
                    AccountStatusHistory = _dbContext.UserArchives
                        .Where(a => a.TargetId == userId)
                        .Select(a => new { a.Type, a.Reason, a.CreatedAt })
                        .ToList()
                })
                .FirstOrDefaultAsync(CancellationToken.None);

            if (userData == null)
            {
                exportRequest.Fail();
                await _unitOfWork.CompleteAsync(CancellationToken.None);
                return;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            jsonOptions.Converters.Add(new JsonStringEnumConverter());

            string jsonString = JsonSerializer.Serialize(userData, jsonOptions);

            string tempFolder = Path.Combine(Path.GetTempPath(), "SNS_GDPR_Exports");
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);

            jsonPath = Path.Combine(tempFolder, $"data_{userId}.json");
            zipPath = Path.Combine(tempFolder, $"GDPR_Export_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}.zip");

            await File.WriteAllTextAsync(jsonPath, jsonString, CancellationToken.None);

            if (File.Exists(zipPath)) File.Delete(zipPath);
            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(jsonPath, "my_personal_data.json");
            }

            string objectKey = $"gdpr-exports/{userId}/{Path.GetFileName(zipPath)}";
            string finalDownloadUrl = string.Empty;

            using (var fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                finalDownloadUrl = await _fileStorageService.UploadFileAsync(
                    fileStream,
                    "application/zip",
                    objectKey,
                    CancellationToken.None);
            }

            exportRequest.Complete(finalDownloadUrl);
            await _unitOfWork.CompleteAsync(CancellationToken.None);
            
            try
            {
                var template = await _emailTemplateProvider.ReadTemplate(
                    userData.AccountInformation.PreferredLanguage,
                    SendPurpose.ExportDataCompleted,
                    new List<MessageReplacement> 
                    { 
                        new MessageReplacement 
                        (
                            Key : ReplacementKey.RedirectUrl, 
                            Value : _fileStorageService.GetFilePublicUrl(objectKey)  
                        )
                    });

                await _emailSenderService.SendEmailAsync(
                    toEmail: userData.AccountInformation.Email,
                    subject: template.Subject,
                    message: template.Body);
            }
            catch
            {

            }
        }
        catch (Exception)
        {
            exportRequest.Fail();
            await _unitOfWork.CompleteAsync(CancellationToken.None);
        }
        finally
        {
            if (!string.IsNullOrEmpty(jsonPath) && File.Exists(jsonPath)) File.Delete(jsonPath);
            if (!string.IsNullOrEmpty(zipPath) && File.Exists(zipPath)) File.Delete(zipPath);
        }
    }
}