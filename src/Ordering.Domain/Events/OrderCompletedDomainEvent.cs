namespace Ordering.Domain.Events
{
    public class OrderCompletedDomainEvent : INotification
    {
        public Order Order;

        public OrderCompletedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}
