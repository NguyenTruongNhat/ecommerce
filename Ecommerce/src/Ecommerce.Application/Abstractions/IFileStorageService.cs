using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage.Models;
using static Ecommerce.Contract.Services.V1.FileStorage.Command;
using static Ecommerce.Contract.Services.V1.FileStorage.Response;

namespace Ecommerce.Application.Abstractions;
public interface IFileStorageService
{
    Task<Result<List<S3ObjectDto>>> UploadMultipleFilesToCloudAsync(UploadMultipleFilesCommand dto);
    Task<MemoryStream> DownloadFileCloudAsync(string objectName);
    Task RenameS3File(string sourceName, string targetName);
    Task<Result<string>> UploadImageFile(UploadLogoCommand request);
    Task<Result<UploadAvatarResponseDto>> UploadAvatar(UploadAvatarCommand request);
    Task CreateThumbnail(string signedUrl, string objectName);
    CreateUploadSignedUrlResponseDto CreateSignedUrlForUpload(CreateUploadSignedUrlCommand dto);
    CreateUploadSignedUrlResponseDto CreateSignedUrlForMultipartUpload(CreateMultipartUploadSignedUrlCommand dto);
    Task<CreateMultipartUploadResponseDto> CreateMultipartUploadId(CreateMultipartUploadCommand dto);
    Task<bool> CompleteMultipartUpload(CompleteMultipartUploadCommand dto);
    Task<bool> AbortMultipartUpload(CompleteMultipartUploadCommand dto);


}
