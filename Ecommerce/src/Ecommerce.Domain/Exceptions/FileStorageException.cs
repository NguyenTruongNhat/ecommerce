namespace Ecommerce.Domain.Exceptions;

public static class FileStorageException
{
    public class ItemNotFoundException : NotFoundException
    {
        public ItemNotFoundException(int Id)
            : base($"The item with the id {Id} was not found.") { }
    }
}
