using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contract.Services.V1.FileStorage;
public static class Response
{
    public record CreateMultipartUploadResponseDto
    {
        public string UploadId { get; set; }
        public string ObjectName { get; set; }
        public string FileType { get; set; }
    }

    public record CreateUploadSignedUrlResponseDto
    {
        public string SignedUrl { get; set; }
        public string ContentType { get; set; }
        public string ObjectName { get; set; }
        public string FileType { get; set; }
    }

    public record UploadAvatarResponseDto
    {
        public string ImageName { get; set; }
        public string ObjectName { get; set; }
    }

    public record PreSignedUrlResponseDto
    {
        public string SignedUrl { get; set; }
        public string ObjectName { get; set; }
        public string FileName { get; set; }
        public int ExpiresInMinutes { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public record UploadLogoResponseDto
    {
        public string ResourceId { get; set; }
        public string ObjectName { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string DownloadUrl { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public record UploadMultipleFilesResponseDto
    {
        public string UploadingProgressId { get; set; }
        public int TotalFiles { get; set; }
        public int SuccessfulUploads { get; set; }
        public int FailedUploads { get; set; }
        public List<FileUploadResultDto> Results { get; set; } = new();
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public double TotalDurationSeconds { get; set; }
    }

    public record FileUploadResultDto
    {
        public string FileName { get; set; }
        public string ObjectName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string DownloadUrl { get; set; }
    }

    public record UploadProgressDto
    {
        public string UploadingProgressId { get; set; }
        public string FileName { get; set; }
        public int FileIndex { get; set; }
        public int TotalFiles { get; set; }
        public long TransferredBytes { get; set; }
        public long TotalBytes { get; set; }
        public double PercentComplete { get; set; }
        public string Status { get; set; } // "uploading", "completed", "failed"
    }

    public record InitiateMultipartUploadResponseDto
    {
        public string UploadId { get; set; }
        public string ObjectName { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FileType { get; set; }
        public DateTime InitiatedAt { get; set; }
    }

    public record MultipartUploadSignedUrlResponseDto
    {
        public string SignedUrl { get; set; }
        public string UploadId { get; set; }
        public string ObjectName { get; set; }
        public int PartNumber { get; set; }
        public int ExpiresInMinutes { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public record CompleteMultipartUploadResponseDto
    {
        public string ObjectName { get; set; }
        public string UploadId { get; set; }
        public string ETag { get; set; }
        public string Location { get; set; }
        public int TotalParts { get; set; }
        public string DownloadUrl { get; set; }
        public DateTime CompletedAt { get; set; }
    }

    public record AbortMultipartUploadResponseDto
    {
        public string ObjectName { get; set; }
        public string UploadId { get; set; }
        public string Message { get; set; }
        public DateTime AbortedAt { get; set; }
    }
}
