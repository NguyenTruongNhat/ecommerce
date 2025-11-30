namespace Ecommerce.Domain.Exceptions;
public static class CommonException
{
    public class NotFound : NotFoundException
    {
        public NotFound() : base($"The item not found") { }
    }

    public class AlreadyExists : BadRequestException
    {
        public AlreadyExists() : base($"The item is already") { }
    }
}
