using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage.Models;
using static Ecommerce.Contract.Services.V1.FileStorage.Command;
using static Ecommerce.Contract.Services.V1.FileStorage.Response;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Abstractions;

/// <summary>
/// Abstraction for file storage operations using cloud storage services
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file directly to storage
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="objectName">The unique name/key of the object in storage</param>
    /// <param name="contentType">The MIME type of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload result with object URL</returns>
    Task<string> UploadFileAsync(
        IFormFile file,
        string objectName,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a file with progress tracking using TransferUtility
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="objectName">The unique name/key of the object in storage</param>
    /// <param name="contentType">The MIME type of the file</param>
    /// <param name="progressCallback">Callback for progress updates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload result with object URL</returns>
    Task<string> UploadFileWithProgressAsync(
        IFormFile file,
        string objectName,
        string contentType,
        Action<long, long> progressCallback,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a multipart upload and returns the upload ID
    /// </summary>
    /// <param name="objectName">The unique name/key of the object in storage</param>
    /// <param name="contentType">The MIME type of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload ID for the multipart upload session</returns>
    Task<string> InitiateMultipartUploadAsync(
        string objectName,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a presigned URL for uploading a specific part in a multipart upload
    /// </summary>
    /// <param name="objectName">The unique name/key of the object in storage</param>
    /// <param name="uploadId">The upload ID from InitiateMultipartUpload</param>
    /// <param name="partNumber">The part number (1-indexed, max 10000)</param>
    /// <param name="expiresInMinutes">URL expiration time in minutes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Presigned URL for uploading the part</returns>
    Task<string> GeneratePresignedUrlForPartAsync(
        string objectName,
        string uploadId,
        int partNumber,
        int expiresInMinutes = 60,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes a multipart upload by combining all uploaded parts
    /// </summary>
    /// <param name="objectName">The unique name/key of the object in storage</param>
    /// <param name="uploadId">The upload ID from InitiateMultipartUpload</param>
    /// <param name="partETags">List of ETags for all uploaded parts</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing ETag and location of the completed object</returns>
    Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(
        string objectName,
        string uploadId,
        List<PartETag> partETags,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aborts a multipart upload and removes all uploaded parts
    /// </summary>
    /// <param name="objectName">The unique name/key of the object in storage</param>
    /// <param name="uploadId">The upload ID from InitiateMultipartUpload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AbortMultipartUploadAsync(
        string objectName,
        string uploadId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a pre-signed URL for uploading a file
    /// </summary>
    /// <param name="objectName">The unique name/key of the object in storage</param>
    /// <param name="contentType">The MIME type of the file</param>
    /// <param name="expiresInMinutes">URL expiration time in minutes (default: 60)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pre-signed URL for upload</returns>
    Task<string> GeneratePresignedUploadUrlAsync(
        string objectName,
        string contentType,
        int expiresInMinutes = 60,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a pre-signed URL for downloading a file
    /// </summary>
    /// <param name="objectName">The unique name/key of the object in storage</param>
    /// <param name="expiresInMinutes">URL expiration time in minutes (default: 60)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pre-signed URL for download</returns>
    Task<string> GeneratePresignedDownloadUrlAsync(
        string objectName,
        int expiresInMinutes = 60,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a unique object name with proper path structure
    /// </summary>
    /// <param name="appServiceName">The application service/folder name</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="uploadingProgressId">Unique identifier for the upload session</param>
    /// <returns>Structured object name</returns>
    string GenerateObjectName(string appServiceName, string fileName, string uploadingProgressId);

    /// <summary>
    /// Gets the content type from file extension
    /// </summary>
    /// <param name="fileName">File name with extension</param>
    /// <returns>MIME content type</returns>
    string GetContentType(string fileName);
}
