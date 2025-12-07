namespace Ecommerce.Contract.Services.V1.FileStorage.Models;
public class S3ObjectDto
{
    public long FileSize { get; set; }
    public int Duration { get; set; }
    public string Type { get; set; }
    public string ObjectName { get; set; }
    public string FileName { get; set; }
}
