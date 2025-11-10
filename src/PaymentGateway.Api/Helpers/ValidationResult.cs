namespace PaymentGateway.Api.Helpers
{

    // simple validator model, returns dynamic value if the validation is successful or a list of errors if it fails
    public class ValidationResult<T>
    {
        public bool IsValid { get; set; }
        public T? Value { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        private ValidationResult() { }


        public static ValidationResult<T> Success(T value)
        {
            return new ValidationResult<T>
            {
                IsValid = true,
                Value = value
            };
        }

        public static ValidationResult<T> Fail(List<string> errors)
        {
            return new ValidationResult<T> 
            {
                IsValid = false,
                Value = default, 
                Errors = errors
            };
        }

        public static ValidationResult<T> Fail(List<string> errors, T value)
        {
            return new ValidationResult<T>
            {
                IsValid = false,
                Value = value,
                Errors = errors
            };
        }


        public void AddError(string message)
        {
            Errors.Add(message);
        }

    }
}
