﻿using Ordering.Domain.Events;

namespace eShop.Ordering.API.Application.DomainEventHandlers
{
    public class OrderCompletedDomainEventHandler : INotificationHandler<OrderCompletedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly ILogger _logger;

        public OrderCompletedDomainEventHandler(IOrderRepository orderRepository, IBuyerRepository buyerRepository, IOrderingIntegrationEventService orderingIntegrationEventService, ILogger<OrderCompletedDomainEventHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _orderingIntegrationEventService = orderingIntegrationEventService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            OrderingApiTrace.LogOrderStatusUpdated(_logger, domainEvent.Order.Id, OrderStatus.Completed);

            var order = await _orderRepository.GetAsync(domainEvent.Order.Id);
            var buyer = await _buyerRepository.FindByIdAsync(order.BuyerId.Value);

            var integrationEvent = new OrderStatusChangedToCompletedIntegrationEvent(order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
            await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
        }
    }
}
