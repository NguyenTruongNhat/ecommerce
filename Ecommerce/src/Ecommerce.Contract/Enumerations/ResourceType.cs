using Ardalis.SmartEnum;

namespace Ecommerce.Contract.Enumerations;
public sealed class ResourceType : SmartEnum<ResourceType, int>
{
    public static ResourceType Audio = new ResourceType(nameof(Audio), 1);
    public static ResourceType Video = new ResourceType(nameof(Video), 2);
    public static ResourceType Document = new ResourceType(nameof(Document), 3);
    public ResourceType(string name, int value) : base(name, value)
    {

    }
}
