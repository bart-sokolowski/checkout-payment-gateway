using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Helpers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = _paymentService.GetPaymentById(id);

        return new OkObjectResult(payment);
    }

    [HttpPost]
    public async Task<ActionResult> PostPaymentAsync([FromBody] PostPaymentRequest paymnetRequest)
    {
        PostPaymentResponse paymentResponse = await _paymentService.ProcessPaymentAsync(paymnetRequest);

        return new OkObjectResult(paymentResponse);
    }
}