namespace Ecommerce.Infrastructure.DependencyInjection.Options;
public class AwsS3Options
{
    public string Endpoint { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string AccessKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;

    public string BucketName { get; set; } = default!;
}
