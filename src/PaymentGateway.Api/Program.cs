using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

//Add base bank url (simulate getting external env variable)
if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("BANK_SIMULATOR_URL")))
{
    Environment.SetEnvironmentVariable("BANK_SIMULATOR_URL", "http://localhost:8080");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<PaymentsRepository>();
builder.Services.AddScoped<BankService>();
builder.Services.AddScoped<PaymentService>();

var bankUrl = Environment.GetEnvironmentVariable("BANK_SIMULATOR_URL");

builder.Services.AddHttpClient<BankService>(client =>
{
    client.BaseAddress = new Uri(bankUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }