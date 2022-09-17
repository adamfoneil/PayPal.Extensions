using PayPal.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddLogging(config => config.AddConsole());

var app = builder.Build();

app.UseHttpsRedirection();

// requires
// - a stable Ngrok url to test, https://ngrok.com/
// - a PayPal account for use with https://developer.paypal.com/developer/ipnSimulator

app.MapPost("/IpnTest", async (HttpContext context) =>
{
    var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("IpnTest");

    try
    {
        var result = await context.Request.VerifyPayPalTransactionAsync(PayPalEnvironment.Sandbox, httpClientFactory, logger);

        if (result.IsVerified)
        {
            logger.LogInformation($"Item num: {result.Transaction.ItemNumber}, gross = {result.Transaction.Gross:c2}");
        }
    }
    catch (Exception exc)
	{
        // we don't exception to escape from this handler
        logger.LogError(exc, "Error in /IpnTest handler: {message}", exc.Message);
	}
});

app.MapGet("/", async (HttpResponse response) =>
{
    await response.WriteAsync("Hello");
});

app.Run();