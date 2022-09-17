using System.Text.Json.Serialization;

namespace PayPalExtensions.Models
{
    /// <summary>
    /// based on info here: https://developer.paypal.com/api/nvp-soap/ipn/IPNandPDTVariables/
    /// </summary>
    public class PayPalTransaction
    {
        [JsonPropertyName("txn_type")]
        public string? TransactionType { get; set; }

        [JsonPropertyName("txn_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("parent_txn_id")]
        public string? ParentTransactionId { get; set; }

        [JsonPropertyName("business")]
        public string? Business { get; set; }

        [JsonPropertyName("receiver_email")]
        public string? ReceiverEmail { get; set; }

        [JsonPropertyName("address_country")]
        public string? AddressCountry { get; set; }

        [JsonPropertyName("address_city")]
        public string? AddressCity { get; set; }

        [JsonPropertyName("address_country_code")]
        public string? AddressCountryCode { get; set; }

        [JsonPropertyName("address_name")]
        public string? AddressName { get; set; }

        [JsonPropertyName("address_state")]
        public string? AddressState { get; set; }

        [JsonPropertyName("address_street")]
        public string? AddressStreet { get; set; }

        [JsonPropertyName("address_zip")]
        public string? AddressZip { get; set; }

        [JsonPropertyName("contact_phone")]
        public string? ContactPhone { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("payer_business_name")]
        public string? PayerBusinessName { get; set; }

        [JsonPropertyName("payer_email")]
        public string? PayerEmail { get; set; }

        [JsonPropertyName("payer_id")]
        public string? PayerId { get; set; }

        [JsonPropertyName("item_number")]
        public string? ItemNumber { get; set; }

        [JsonPropertyName("item_name")]
        public string? ItemName { get; set; }

        [JsonPropertyName("mc_fee")]
        public string? FeeStr { get; set; }

        public decimal Fee => decimal.TryParse(FeeStr, out decimal val) ? val : 0;

        [JsonPropertyName("mc_gross")]
        public string? GrossStr { get; set; }

        public decimal Gross => decimal.TryParse(GrossStr, out decimal val) ? val : 0;

        public decimal Net { get { return Gross - Fee; } }

        [JsonPropertyName("memo")]
        public string? Memo { get; set; }

        [JsonPropertyName("payment_status")]
        public string? PaymentStatus { get; set; }

        [JsonPropertyName("payment_type")]
        public string? PaymentType { get; set; }
    }
}
