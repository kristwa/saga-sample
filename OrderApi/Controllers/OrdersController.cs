using Common.DTO;
using Common.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace OrderApi.Controllers;

[Route("[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrdersController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    // Create a new order "the proper way". Publish to trigger Card Saga for each card in order, this test assumes one card per order
    // After all cards are processed, the second saga should trigger and run to completion
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        await _publishEndpoint.Publish<ProcessGiftcardItemRequestCreated>(new ProcessGiftcardItemRequestCreated()
        {
            CorrelationId = itemId,
            GiftcardItem = new()
            {
                Amount = 500,
                ItemId = itemId,
                OrderId = orderId,
            }
        });

        return Ok(new { orderId, itemId });
    }
    
}