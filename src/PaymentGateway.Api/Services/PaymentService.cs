using PaymentGateway.Api.Helpers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services
{
    public class PaymentService
    {
        private readonly PaymentsRepository _paymentsRepository;
        private readonly BankService _bankService;


        public PaymentService(PaymentsRepository paymentsRepository, BankService bankService)
        {
            _paymentsRepository = paymentsRepository;
            _bankService = bankService;
        }

        public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest paymentRequest)
        {
            PostPaymentResponse paymentResponse;

            //validate payment details
            ValidationResult<PostPaymentRequest> validationResult = PaymentValidator.Validate(paymentRequest);

            //if invalid, set the status to rejected -> return
            if (!validationResult.IsValid)
            {
                paymentResponse = Worker.MapPaymnetResponseModel(paymentRequest, Enums.PaymentStatus.Rejected);
                //log the validation errors 
            }

            //if valid, send the payment request to the bank service
            else
            {
                var bankServiceResponse = await _bankService.ProcessPaymentAsync(paymentRequest);
                if (bankServiceResponse.Authorized == true)
                {
                    paymentResponse = Worker.MapPaymnetResponseModel(paymentRequest, Enums.PaymentStatus.Authorized);
                }
                else
                {
                    paymentResponse = Worker.MapPaymnetResponseModel(paymentRequest, Enums.PaymentStatus.Declined);
                }
            }

            // store the payment
            _paymentsRepository.Add(paymentResponse);
            return paymentResponse;
        }

        public PostPaymentResponse? GetPaymentById(Guid id)
        {
            return _paymentsRepository.Get(id);
        }
    }
}
