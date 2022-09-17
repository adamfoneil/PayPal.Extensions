This is a reboot of my [PayPalHelper2](https://github.com/adamfoneil/PayPalHelper2) as several things about that were outdated. I wanted to modernize this for .NET6, leveraging Minimal APIs for testing, removing the Newtonsoft dependency along with some other minor enhancements.

Note that I don't really have a proper integration test because you need your own [Ngrok](https://ngrok.com/) and PayPal accounts to test this effectively. And my testing was against the PayPal sandbox only, not an actual transaction. Still, the heart of this is the [VerifyPayPalTransactionAsync](https://github.com/adamfoneil/PayPal.Extensions/blob/master/PayPal.Extensions/PayPalExtensions.cs) method, used in my test handler [here](https://github.com/adamfoneil/PayPal.Extensions/blob/master/IpnTest/Program.cs#L16-L35).

```csharp
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
```

Note that [this SO Q&A was helpful](https://stackoverflow.com/questions/72579605/accept-x-www-form-urlencoded-in-minimal-api-net-6) because Minimal APIs don't support `application/x-www-form-urlencoded` posts directly, which is what PayPal uses when posting to your IPN listener.
