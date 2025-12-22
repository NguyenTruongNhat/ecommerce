using Ecommerce.Contract.Enumerations;

namespace Ecommerce.Contract.Constants;
public static class FileStorageVariables
{

    public const string LogoFile = "LogoFiles";
    public const string VideosHistory = "VideosHistory";
    public const string TranslatedDocument = "TranslatedDocument";
    public const string OriginalResource = "OriginalResources";
    public const string TempTranscribe = "TempTranscribe";
    public const string ImageLibraries = "ImageLibraries";
    public const string WaveformFile = "Waveforms";
    public const int SignedURLLimitedTime = 3600;
    public const int SignedImageUrlLimitedTime = 21600;
    public const string DownloadableContentType = "application/octet-stream";
    public const string JsonContentType = "application/json";
    public const string SvgContentType = "image/svg+xml";
    public const string CsvContentType = "text/csv";
    public static readonly string[] VideoFileExtensions = { ".mp4", ".mov", ".wmv", ".avi", ".mkv" };
    public static readonly string[] NeedToConvertExtensions = { ".mov", ".wmv", ".avi", ".mkv" };
    public static readonly string[] ImageFileExtensions = { ".jpeg", ".png", ".jpg", ".jfif", ".svg" };
    public static readonly string[] AudioFileExtensions = { ".mp3", ".wav", ".flac" };
    public static readonly string Mp4Extension = ".mp4";
    public static readonly string Mp3Extension = ".mp3";
    public static readonly string CsvExtension = ".csv";
    public static readonly string[] DocumentFileExtensions = {  ".txt",
                                                                ".docx",
                                                                ".pptx",
                                                                ".xlsx",
                                                                ".msg",
                                                                ".html",
                                                                ".htm",
                                                                ".pdf",
                                                                ".xlf",
                                                                ".tsv",
                                                                ".tab",
                                                                ".csv",
                                                                ".rtf",
                                                                ".doc",
                                                                ".ppt",
                                                                ".xls",
                                                                ".odt",
                                                                ".odp",
                                                                ".ods",
                                                                ".markdown",
                                                                ".mdown",
                                                                ".mkdn",
                                                                ".md",
                                                                ".mkd",
                                                                ".mdwn",
                                                                ".mdtxt",
                                                                ".mdtext",
                                                                ".rmd",
                                                                ".mhtml",
                                                                ".mht" };
    public static readonly Dictionary<ResourceType, string[]> DictFileTypeExtensions = new Dictionary<ResourceType, string[]>
    {
        {ResourceType.Audio, AudioFileExtensions },
        {ResourceType.Video, VideoFileExtensions },
        {ResourceType.Document, DocumentFileExtensions },
    };
}
