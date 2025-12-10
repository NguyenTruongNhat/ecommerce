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
}
