using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Contracts;

namespace SNS.Application.Identity.ArchiveManagement.Commands.ExportAccountData;


public sealed record ExportAccountDataCommand : ICommand<ExportAccountDataResponseDto>;