using Asp.Versioning;
using Ecommerce.Contract.Services.V1.FileStorage;
using Ecommerce.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Presentation.Controllers.V1;

[ApiVersion(1)]
public class FileStorageController : ApiController

{
    public FileStorageController(ISender sender) : base(sender)
    {
    }

    [HttpPost("multiple-upload")]
    public async Task<IActionResult> Upload([FromForm] Command.UploadMultipleFilesCommand request)
    {
        //var result = await _awsStorageService.UploadMultipleFilesToCloudAsync(request);
        var result = await Sender.Send(request);


        return Ok(result);
    }

    [HttpPost("image-upload")]
    public async Task<IActionResult> UploadImage([FromForm] Command.UploadLogoCommand request)
    {
        //var result = await _awsStorageService.UploadImageFile(request);
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpGet]
    [Route("upload-signed-url")]
    public async Task<IActionResult> GetPreSignedUrlForUpload([FromQuery] Command.CreateUploadSignedUrlCommand request)
    {
        //var signedUrl = _awsStorageService.CreateSignedUrlForUpload(dto);
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpGet]
    [Route("multipart-upload-id")]
    public async Task<IActionResult> CeateMultipartUpload([FromQuery] Command.CreateMultipartUploadCommand request)
    {
        //var signedUrl = await _awsStorageService.CreateMultipartUploadId(dto);
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpGet]
    [Route("multipart-upload-signed-url")]
    public async Task<IActionResult> GetPreSignedUrlForMultipartUpload([FromQuery] Command.CreateMultipartUploadSignedUrlCommand request)
    {
        //var signedUrl = _awsStorageService.CreateSignedUrlForMultipartUpload(dto);
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpPost]
    [Route("complete-multipart")]
    public async Task<IActionResult> ComleteMultipartUpload([FromBody] Command.CompleteMultipartUploadCommand request)
    {
        //var signedUrl = _awsStorageService.CompleteMultipartUpload(dto);
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpPost]
    [Route("abort-multipart")]
    public async Task<IActionResult> AbortMultipartUpload([FromBody] Command.AbortMultipartUploadCommand request)
    {
        //var signedUrl = _awsStorageService.AbortMultipartUpload(dto);
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpGet]
    [Route("signed-url")]
    public async Task<IActionResult> GetPreSignedUrl([FromQuery] Command.PreSignedUrlCommand request)
    {
        //string signedUrl = _awsStorageService.CreateSignedUrlWithFileName(objectName, fileName);
        var result = await Sender.Send(request);
        return Ok(result);
    }

    [HttpPost]
    [Route("avatar-upload")]
    public async Task<IActionResult> UploadAvatar([FromForm] Command.UploadAvatarCommand request)
    {
        //var result = await _awsStorageService.UploadAvatar(request);
        var result = await Sender.Send(request);
        return Ok(result);
    }

}
