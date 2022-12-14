using PayPal.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddLogging(config => config.AddConsole());

var app = builder.Build();

app.UseHttpsRedirection();

// requires
// - an Ngrok url to test, https://ngrok.com/
// - a PayPal account for use with https://developer.paypal.com/developer/ipnSimulator

app.MapPost("/IpnTest", async (HttpContext context) =>
{
    // for efficient HttpClient use
    var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();

    // for emitting internal info while it runs
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("IpnTest");

    try
    {
        var result = await context.Request.VerifyPayPalTransactionAsync(PayPalEnvironment.Sandbox, httpClientFactory, logger);

        if (result.IsVerified)
        {
            // should be values you entered in IPN simulator form
            logger.LogInformation($"Item num: {result.Transaction.ItemNumber}, gross = {result.Transaction.Gross:c2}");
        }
    }
    catch (Exception exc)
    {
        // we don't want exception to escape from this handler
        logger.LogError(exc, "Error in /IpnTest handler: {message}", exc.Message);
    }
});

app.MapGet("/", async (HttpResponse response) =>
{
    await response.WriteAsync("Hello");
});

app.Run();