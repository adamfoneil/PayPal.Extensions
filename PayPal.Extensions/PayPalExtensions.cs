using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PayPalExtensions.Models;
using System.Net.Http;
using System.Text.Json;

namespace PayPal.Extensions
{
    public enum PayPalEnvironment
    {
        Sandbox,
        Live
    }    

    public static class PayPalExtensions
    {
        public static Dictionary<PayPalEnvironment, Uri> Urls => new()
        {
            [PayPalEnvironment.Live] = new Uri("https://ipnpb.paypal.com/"),
            [PayPalEnvironment.Sandbox] = new Uri("https://ipnpb.sandbox.paypal.com/")
        };

        public static async Task<VerificationResult> VerifyPayPalTransactionAsync(this HttpRequest request, PayPalEnvironment environment, IHttpClientFactory httpClientFactory, ILogger? logger = null)
        {
            VerificationResult result = new();

            using var client = httpClientFactory.CreateClient();

            try
            {                
                if (client.BaseAddress == null)
                {
                    client.BaseAddress = Urls[environment];                    
                }                

                client.DefaultRequestHeaders.Accept.Clear();

                var formValues = new Dictionary<string, string>();
                formValues.Add("cmd", "_notify-validate");
                foreach (var field in request.Form) formValues.Add(field.Key, field.Value);
                var content = new FormUrlEncodedContent(formValues);

                var response = await client.PostAsync("cgi-bin/webscr", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    result.IsVerified = responseText.Trim().Equals("VERIFIED");

                    if (result.IsVerified)
                    {
                        result.Transaction = request.Form.ParseForm<PayPalTransaction>();
                    }
                    
                    return result;
                }
                
                throw new Exception($"PayPal API call failed: {response.Content}");                
            }
            catch (Exception exc)
            {
                result.Exception = exc;
                logger?.LogError(exc, "Error in VerifyPayPalTransactionAsync: {message}", exc.Message);
            }

            return result;
        }

        public static T? ParseForm<T>(this IFormCollection form) where T : new()
        {
            var fields = form
                .Select(keyPair => new KeyValuePair<string, string>(keyPair.Key, keyPair.Value.First()))
                .ToDictionary(keyPair => keyPair.Key, keyPair => keyPair.Value);

            string json = JsonSerializer.Serialize(fields);

            if (string.IsNullOrEmpty(json)) return default;

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}