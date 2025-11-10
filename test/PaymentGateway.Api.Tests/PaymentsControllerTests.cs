using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();

    #region GetPayment

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP"
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region PostPayment

    //payment validation tests
    [Fact]
    public async Task ReturnsRejectedPaymentDueToInvalidPaymentFields()
    {
        var currentYear = DateTime.UtcNow.Year;
        var currentMonth = DateTime.UtcNow.Month;

        var invalidPayments = new List<PostPaymentRequest>
    {

        // invalid card number 
        new PostPaymentRequest
        {
            CardNumber = "1234",
            ExpiryMonth = currentMonth,
            ExpiryYear = currentYear + 1,
            Cvv = "123",
            Amount = 100,
            Currency = "USD"
        },

        // invalid expiry month
        new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 0,
            ExpiryYear = currentYear + 1,
            Cvv = "123",
            Amount = 100,
            Currency = "USD"
        },

        // invalid expiry year
        new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = currentMonth,
            ExpiryYear = currentYear - 1,
            Cvv = "123",
            Amount = 100,
            Currency = "USD"
        },

        // invalid CVV
        new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = currentMonth,
            ExpiryYear = currentYear + 1,
            Cvv = "1",
            Amount = 100,
            Currency = "USD"
        },

        // invalid amount
        new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = currentMonth,
            ExpiryYear = currentYear + 1,
            Cvv = "123",
            Amount = 0,
            Currency = "USD"
        },

        // invalid currency
        new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = currentMonth,
            ExpiryYear = currentYear + 1,
            Cvv = "123",
            Amount = 100,
            Currency = "USDX"
        }
    };


        var factory = new WebApplicationFactory<Program>();

        var client = factory.CreateClient();


        foreach (var invalidRequest in invalidPayments)
        {
            var response = await client.PostAsJsonAsync("/api/Payments", invalidRequest);
            var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

            Assert.NotNull(paymentResponse);
            Assert.Equal(PaymentStatus.Rejected, paymentResponse.Status);
        }
    }

    [Fact]
    public async Task ReturnsAuthorizedPayment()
    {
        var currentYear = DateTime.UtcNow.Year;
        var currentMonth = DateTime.UtcNow.Month;

        var paymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = currentMonth,
            ExpiryYear = currentYear,
            Cvv = "123",
            Amount = 100,
            Currency = "USD"
        };

        var factory = new WebApplicationFactory<Program>();

        var client = factory.CreateClient();


        var response = await client.PostAsJsonAsync("/api/Payments", paymentRequest);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse.Status);
    }

    [Fact]
    public async Task ReturnsDeclinedPaymentDueToBankSystemFailure()
    {
        var currentYear = DateTime.UtcNow.Year;
        var currentMonth = DateTime.UtcNow.Month;

        //set card number to finish with "0"
        var paymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248870",
            ExpiryMonth = currentMonth,
            ExpiryYear = currentYear + 1,
            Cvv = "123",
            Amount = 100,
            Currency = "USD"
        };

        var factory = new WebApplicationFactory<Program>();

        var client = factory.CreateClient();


        var response = await client.PostAsJsonAsync("/api/Payments", paymentRequest);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Declined, paymentResponse.Status);
    }

    [Fact]
    public async Task ReturnsDeclinedPaymentDueToBankResponse()
    {
        var currentYear = DateTime.UtcNow.Year;
        var currentMonth = DateTime.UtcNow.Month;

        //set card number to finish with even number
        var paymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248878",
            ExpiryMonth = currentMonth,
            ExpiryYear = currentYear,
            Cvv = "123",
            Amount = 100,
            Currency = "USD"
        };

        var factory = new WebApplicationFactory<Program>();

        var client = factory.CreateClient();


        var response = await client.PostAsJsonAsync("/api/Payments", paymentRequest);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Declined, paymentResponse.Status);
    }

    #endregion
}
