using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace LegacyInventory.Web.Services
{
    public class ShippingIntegrationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShippingIntegrationService> _logger;

        public ShippingIntegrationService(IConfiguration configuration, ILogger<ShippingIntegrationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private HttpClient GetHttpClient()
        {
            return new HttpClient();
        }

        public ShipmentTrackingInfo GetTrackingInfo(int shipmentId)
        {
            var client = new RestClient(_configuration["Shipping:ApiBaseUrl"] ?? "https://shipping.example.com/api");
            var request = new RestRequest("shipments/{id}", Method.GET);
            request.AddUrlSegment("id", shipmentId);
            request.AddHeader("Authorization", $"Bearer {_configuration["Shipping:ApiKey"]}");
            var response = client.Execute<ShipmentTrackingInfo>(request);

            if (response.Data != null)
            {
                return response.Data;
            }

            return new ShipmentTrackingInfo
            {
                ShipmentId = shipmentId,
                Status = response.IsSuccessful ? "Pending" : "Unavailable",
                CarrierName = "Legacy Carrier",
                RawResponse = response.Content ?? string.Empty
            };
        }

        public Task<ShipmentTrackingInfo> GetTrackingInfoAsync(int shipmentId)
        {
            var client = new RestClient(_configuration["Shipping:ApiBaseUrl"] ?? "https://shipping.example.com/api");
            var request = new RestRequest("shipments/{id}", Method.GET);
            request.AddUrlSegment("id", shipmentId);
            request.AddHeader("Authorization", $"Bearer {_configuration["Shipping:ApiKey"]}");
            var response = client.ExecuteAsync(request).Result;

            _logger.LogInformation("Loaded tracking response for shipment {ShipmentId}", shipmentId);

            return Task.FromResult(new ShipmentTrackingInfo
            {
                ShipmentId = shipmentId,
                Status = response.IsSuccessful ? "InTransit" : "Unavailable",
                CarrierName = "Legacy Carrier",
                RawResponse = response.Content ?? string.Empty
            });
        }

        public string PushShipmentAudit(ShipmentTrackingInfo trackingInfo)
        {
            var client = GetHttpClient();
            var payload = JsonConvert.SerializeObject(trackingInfo);
            var response = client.PostAsync(
                _configuration["Shipping:AuditEndpoint"] ?? "https://shipping.example.com/audit",
                new StringContent(payload, Encoding.UTF8, "application/json")).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            return body;
        }

        public ShippingPingResult PingApi()
        {
            var client = GetHttpClient();
            var response = client.GetAsync((_configuration["Shipping:ApiBaseUrl"] ?? "https://shipping.example.com/api").TrimEnd('/') + "/ping").Result;
            return new ShippingPingResult
            {
                StatusCode = (int)response.StatusCode,
                Payload = response.Content.ReadAsStringAsync().Result
            };
        }
    }

    public class ShipmentTrackingInfo
    {
        public int ShipmentId { get; set; }

        public string Status { get; set; }

        public string CarrierName { get; set; }

        public string RawResponse { get; set; }
    }

    public class ShippingPingResult
    {
        public int StatusCode { get; set; }

        public string Payload { get; set; }
    }
}
