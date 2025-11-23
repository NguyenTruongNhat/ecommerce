namespace Ecommerce.Contract.Enumerations;
public enum OrderStatus
{
    PENDING_PAYMENT,
    PENDING_PICKUP,
    PENDING_DELIVERY,
    DELIVERED,
    RETURNED,
    CANCELLED
}
