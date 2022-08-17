using Common.DTO;
using Common.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace OrderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CardsController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CardsController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    [HttpPut]
    public async Task<IActionResult> CreateCardOrder(string? cardNo)
    {
        OrderType type = string.IsNullOrEmpty(cardNo) ? OrderType.Purchase : OrderType.Deposit;
        Guid id = Guid.NewGuid();

        await _publishEndpoint.Publish<ProcessGiftcardItemRequestCreated>(new ProcessGiftcardItemRequestCreated()
        {
            CorrelationId = id,
            GiftcardItem = new GiftcardItem()
            {
                Amount = 400,
                CardNumber = cardNo,
                HasPrintDesign = string.IsNullOrEmpty(cardNo),
                ItemId = id,
                OrderId = Guid.NewGuid(),
                OrderType = type
            }
        });

        return Ok(new { id });
    }
    
}