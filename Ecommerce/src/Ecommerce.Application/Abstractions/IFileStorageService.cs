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

/// <summary>
/// Abstraction for file storage operations using cloud storage services
/// </summary>
public interface IFileStorageService
{
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
