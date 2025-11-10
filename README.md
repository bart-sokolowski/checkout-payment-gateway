#  Payment Gateway API

This project implements a simple Payment Gateway API designed with clean architecture principles in mind.  
It provides two main endpoints:

- **POST `/payments`** – processes a new payment request  
- **GET `/payments/{id}`** – retrieves details of a previously created payment  

---

##  Architecture Overview

Core business logic is encapsulated in the `PaymentService`, which coordinates validation, external API communication, and data persistence.

###  PaymentService
- Acts as the central orchestrator connecting other components.  
- Handles the full payment workflow:
  -  Validates incoming payment requests  
  -  Sends valid requests to the Bank API simulator  
  -  Maps the bank’s response to the `PostPaymentResponse` model  
  -  Stores successful transactions in the repository for retrieval  

---

### PaymentsRepository
- A simple in-memory data store used for the purposes of this demo.  
- Handles adding and retrieving payment records.  
- Serves as an abstraction layer that can easily be replaced by a real database context.  

---

### BankService
- Responsible for connecting to the Bank API simulator, which mimics real-world payment authorization behavior:
  - **Authorized** – if the card number ends with an odd digit 
  - **Declined** – if it ends with an even digit 
  - **Unavailable** – if it ends with 0  
- Returns the result of the transaction back to `PaymentService`, which then maps it into the final Payment model.  

---

### Validation Layer
Before contacting the Bank API, each request goes through a dedicated PaymentValidator.

This validator ensures that:
-  Card number is numeric and between 14–19 digits  
-  Expiry month is between 1–12  
-  Expiry date is in the future  
-  Currency is a valid 3-letter ISO code and one of the supported currencies
-  Amount is a positive integer  
-  CVV is 3–4 numeric digits  

The validator returns a `ValidationResult<T>` object containing:
- **IsValid** – indicates success or failure  
- **Errors** – a list of validation issues, if any  
- **Value** – the parsed, validated model  

---

