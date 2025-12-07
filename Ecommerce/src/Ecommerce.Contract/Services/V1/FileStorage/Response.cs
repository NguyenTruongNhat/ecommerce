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

}
