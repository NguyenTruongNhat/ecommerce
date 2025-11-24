namespace Ecommerce.Persistence.Constants;

internal static class TableNames
{
    // *********** Plural Nouns ***********
    internal const string User = nameof(User);
    internal const string Permission = nameof(Permission);
    internal const string PermissionsRoles = nameof(PermissionsRoles);
    internal const string Role = nameof(Role);

    // *********** Singular Nouns ***********
    public const string Product = nameof(Product);
    public const string Language = nameof(Language);
    public const string UserTranslation = nameof(UserTranslation);
    public const string VerificationCode = nameof(VerificationCode);
    public const string Device = nameof(Device);
    public const string RefreshToken = nameof(RefreshToken);
    public const string ProductTranslation = nameof(ProductTranslation);
    public const string Category = nameof(Category);
    public const string CategoryTranslation = nameof(CategoryTranslation);
    public const string SKU = nameof(SKU);
    public const string Brand = nameof(Brand);
    public const string BrandTranslation = nameof(BrandTranslation);
    public const string CartItem = nameof(CartItem);
    public const string ProductSKUSnapshot = nameof(ProductSKUSnapshot);
    public const string Order = nameof(Order);
    public const string Payment = nameof(Payment);
    public const string Websocket = nameof(Websocket);
    public const string Review = nameof(Review);
    public const string ReviewMedia = nameof(ReviewMedia);
    public const string PaymentTransaction = nameof(PaymentTransaction);
    public const string Message = nameof(Message);
}
