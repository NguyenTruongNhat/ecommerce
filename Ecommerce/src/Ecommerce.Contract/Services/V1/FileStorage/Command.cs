using System.ComponentModel.DataAnnotations;
using Amazon.S3.Model;
using Ecommerce.Contract.Abstractions.Message;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Contract.Services.V1.FileStorage;

public record BaseUploadFileDto(
    string AppServiceName = "OriginalResource",
    string FileName = null!,
    string UploadingProgressId = null!
);


public static class Command
{
    public record UploadMultipleFilesCommand : BaseUploadFileDto, ICommand
    {
        public List<IFormFile> Files { get; init; } = new();
    }

    public record CompleteMultipartUploadCommand : ICommand
    {
        public string UploadId { get; set; }
        public string ObjectName { get; set; }
        public List<PartETag> PartETags { get; set; }
    }

    public record AbortMultipartUploadCommand : CompleteMultipartUploadCommand 
    {
    }

    public record CreateMultipartUploadCommand : BaseUploadFileDto , ICommand
    {
    }

    public record CreateMultipartUploadSignedUrlCommand : BaseUploadFileDto, ICommand<Response.CreateMultipartUploadResponseDto>
    {
        public int PartNumber { get; set; }
        public string UploadId { get; set; }
        public string ObjectName { get; set; }
    }

    public record CreateUploadSignedUrlCommand : BaseUploadFileDto, ICommand<Response.CreateUploadSignedUrlResponseDto>
    {

    }

    public record UploadAvatarCommand : ICommand<Response.UploadAvatarResponseDto>
    {
        public string ImageName { get; set; }
        public IFormFile File { get; set; }
    }

    public record UploadFileDto : BaseUploadFileDto, ICommand
    {
        public IFormFile File { get; set; }
    }

    public record UploadLogoCommand : ICommand
    {
        [Required]
        public string ResourceId { get; set; }
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }

    public record PreSignedUrlCommand : ICommand
    {
        public string ObjectName { get; set; }
        public string FileName { get; set; }
    }

}

