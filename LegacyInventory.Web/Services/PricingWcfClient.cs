using System;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LegacyInventory.Web.Services
{
    [ServiceContract]
    public interface IPricingService
    {
        [OperationContract]
        decimal GetCurrentPrice(int productId);
    }

    public class PricingWcfClient
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PricingWcfClient> _logger;

        public PricingWcfClient(IConfiguration configuration, ILogger<PricingWcfClient> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public decimal GetCurrentPrice(int productId)
        {
            try
            {
                var binding = new BasicHttpBinding();
                var endpoint = new EndpointAddress(_configuration["Pricing:WcfEndpoint"] ?? "http://pricing.example.com/PricingService.svc");
                var factory = new ChannelFactory<IPricingService>(binding, endpoint);
                var channel = factory.CreateChannel();
                var price = channel.GetCurrentPrice(productId);
                ((ICommunicationObject)channel).Close();
                factory.Close();
                return price;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falling back to cached product price for {ProductId}", productId);
                return 0m;
            }
        }
    }
}
