using System;
using Microsoft.Extensions.Configuration;

namespace ImageService.Tests.Helpers
{
    /// <summary>
    /// Static helper class for accessing test configuration values,
    /// prioritizing environment variables over appsettings
    /// </summary>
    public static class TestConfigurationHelper
    {
        private static readonly IConfiguration _configuration;
        
        // Static constructor to initialize configuration only once
        static TestConfigurationHelper()
        {
            // Build configuration from appsettings.tst.json and environment variables
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.tst.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
        
        /// <summary>
        /// Gets the Azure Vision API endpoint from configuration or environment variables
        /// Environment variable: AZURE_VISION_ENDPOINT
        /// </summary>
        public static string AzureVisionEndpoint => 
            _configuration["AZURE_VISION_ENDPOINT"] ?? 
            _configuration["AzureVision:Endpoint"] ?? 
            "https://your-vision-service.cognitiveservices.azure.com/";
            
        /// <summary>
        /// Gets the Azure Vision API key from configuration or environment variables
        /// Environment variable: AZURE_VISION_KEY
        /// </summary>
        public static string AzureVisionKey => 
            _configuration["AZURE_VISION_KEY"] ?? 
            _configuration["AzureVision:Key"] ?? 
            string.Empty;
            
        /// <summary>
        /// Determines if the required Azure Vision API credentials are available
        /// </summary>
        public static bool HasAzureCredentials => 
            !string.IsNullOrEmpty(AzureVisionKey) && 
            AzureVisionKey != "your-vision-api-key" &&
            !string.IsNullOrEmpty(AzureVisionEndpoint) &&
            AzureVisionEndpoint != "https://your-vision-service.cognitiveservices.azure.com/";
            
        /// <summary>
        /// Gets the test image URL for burger image
        /// </summary>
        public static string BurgerImageUrl => 
            "https://www.citypng.com/public/uploads/preview/chicken-burger-with-flying-ingredients-hd-transparent-png-701751710853243v9zgwqwepn.png?v=2025032803";
    }
}