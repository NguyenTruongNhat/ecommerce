using System.Text.RegularExpressions;
using Ecommerce.Contract.Constants;
using Ecommerce.Contract.Enumerations;
using FFMpegCore;

namespace Ecommerce.Contract.Helper;
public static class FileStorageFunctionHelper
{
    public static string CreateObjectName(DateTime dateTime, string accountId, string serviceName, string fileFolder, string fileName)
    {
        string newFileFolder = RemoveSpecialCharacter(fileFolder);
        string newFileName = RemoveSpecialCharacterInFileName(fileName);
        string objectName = $"{accountId}/{serviceName}/{dateTime.Year}/{dateTime.Month}/{dateTime.Day}/{dateTime.ToString("yyyyMMddHHmmssfff")}-{newFileFolder}/{newFileName}";
        return objectName;
    }
    public static string CreateImageObjectName(string userId, string fileName)
    {
        var dateTime = DateTime.UtcNow;
        string newFileName = RemoveSpecialCharacterInFileName(fileName);
        return $"{FileStorageVariables.ImageLibraries}/{userId}/{dateTime.ToString("yyyyMMddHHmmssfff")}-{newFileName}";
    }
    public static string CreateResourceObjectName(string accountId, string serviceName, string resourceId, string fileName)
    {
        string newFileName = RemoveSpecialCharacterInFileName(fileName);
        var currentDate = DateTime.UtcNow;
        string objectName = $"{accountId}/{serviceName}/{resourceId}/{currentDate.Year}/{currentDate.Month}/{currentDate.Day}/{currentDate.ToString("yyyyMMddHHmmssfff")}-{newFileName}";
        return objectName;
    }
    public static string GetFileExtension(string fileName)
    {
        string fileExtension = Path.GetExtension(fileName).ToLower();
        if (FileStorageVariables.AudioFileExtensions.Any(fileExtension.Contains))
        {
            return ResourceType.Audio.Name;
        }
        else if (FileStorageVariables.VideoFileExtensions.Any(fileExtension.Contains))
        {
            return ResourceType.Video.Name;
        }
        else if (FileStorageVariables.DocumentFileExtensions.Any(fileExtension.Contains))
        {
            return ResourceType.Document.Name;
        }
        else
        {
            return null;
        }
    }
    public static bool IsImageFile(string fileName)
    {
        string fileExtension = Path.GetExtension(fileName).ToLower();
        if (FileStorageVariables.ImageFileExtensions.Any(fileExtension.Contains))
        {
            return true;
        }
        return false;
    }
    public static int GetMediaDuration(Stream fileStream, string fileType)
    {
        if (fileType == ResourceType.Document.Name)
        {
            return 0;
        }

        var mediaInfo = FFProbe.Analyse(fileStream);
        return (int)mediaInfo.Duration.TotalSeconds;
    }
    public static async Task<int> GetMediaDuration(string objectName, string fileType)
    {
        if (fileType == ResourceType.Document.Name)
        {
            return 0;
        }

        var uri = new Uri(objectName);
        var mediaInfo = await FFProbe.AnalyseAsync(uri);
        return (int)mediaInfo.Duration.TotalSeconds;
    }
    public static string GetUploadContentType(string fileName)
    {
        string fileExtension = Path.GetExtension(fileName).ToLower();
        if (fileExtension.Contains(FileStorageVariables.CsvExtension))
        {
            return FileStorageVariables.CsvContentType;
        }
        if (FileStorageVariables.DocumentFileExtensions.Any(fileExtension.Contains))
        {
            return null;
        }
        return FileStorageVariables.DownloadableContentType;
    }
    private static string RemoveSpecialCharacterInFileName(string fileName)
    {
        string name = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);
        string outputText = RemoveSpecialCharacter(name);
        return $"{outputText}{extension}";
    }
    private static string RemoveSpecialCharacter(string inputText)
    {
        string pattern = "\\W";
        string outputText = Regex.Replace(inputText, pattern, "-");
        return outputText;
    }
}
