using System;
using System.Threading.Tasks;
using Azure.AI.Vision.ImageAnalysis;
using ImageScorer;
using ImageService.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace ImageService.Tests
{
    // Note: This is an integration test that requires actual Azure credentials
    // To run this test, set the following environment variables:
    // - AZURE_VISION_ENDPOINT
    // - AZURE_VISION_KEY
    public class ImageAnalysisIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _burgerImageUrl;

        public ImageAnalysisIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
            
            // Get configuration from helper
            _endpoint = TestConfigurationHelper.AzureVisionEndpoint;
            _apiKey = TestConfigurationHelper.AzureVisionKey;
            _burgerImageUrl = TestConfigurationHelper.BurgerImageUrl;
            
            // Log configuration status
            _output.WriteLine($"Integration test configuration:");
            _output.WriteLine($"- Endpoint: {_endpoint}");
            _output.WriteLine($"- API Key: {(_apiKey.Length > 0 ? "Set" : "Not Set")}");
            _output.WriteLine($"- Test Image: {_burgerImageUrl}");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Analyze_BurgerImage_ReturnsExpectedTags()
        {
            // Skip the test if credentials aren't provided
            if (!TestConfigurationHelper.HasAzureCredentials)
            {
                _output.WriteLine("Test skipped: Azure credentials not configured");
                return;
            }

            // Create real analyzer and service
            var analyzer = new ImageAnalyzer(_endpoint, _apiKey);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ImageAnalysisService>();
            var service = new ImageAnalysisService(analyzer, logger);

            // Act
            var result = service.AnalyzeImage(new Uri(_burgerImageUrl));

            // Assert
            Assert.NotNull(result);
            
            // Output results to test logger
            if (result.Caption != null)
            {
                _output.WriteLine($"Caption: {result.Caption.Text} (Confidence: {result.Caption.Confidence:P2})");
            }

            if (result.Tags != null)
            {
                _output.WriteLine("Tags:");
                foreach (var tag in result.Tags.Values)
                {
                    _output.WriteLine($"- {tag.Name} (Confidence: {tag.Confidence:P2})");
                }
            }

            // Verify burger-related content is detected
            bool hasFoodTag = false;
            bool hasBurgerTag = false;

            if (result.Tags != null)
            {
                foreach (var tag in result.Tags.Values)
                {
                    if (tag.Name.ToLower().Contains("food"))
                    {
                        hasFoodTag = true;
                    }
                    if (tag.Name.ToLower().Contains("burger") || 
                        tag.Name.ToLower().Contains("sandwich") || 
                        tag.Name.ToLower().Contains("hamburger"))
                    {
                        hasBurgerTag = true;
                    }
                }
            }

            Assert.True(hasFoodTag, "Image should be tagged as food");
            Assert.True(hasBurgerTag, "Image should be tagged as burger/sandwich/hamburger");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AnalyzeAsync_BurgerImage_ReturnsExpectedObjects()
        {
            // Skip the test if credentials aren't provided
            if (!TestConfigurationHelper.HasAzureCredentials)
            {
                _output.WriteLine("Test skipped: Azure credentials not configured");
                return;
            }

            // Create real analyzer and service
            var analyzer = new ImageAnalyzer(_endpoint, _apiKey);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ImageAnalysisService>();
            var service = new ImageAnalysisService(analyzer, logger);

            // Act
            var result = await service.AnalyzeImageAsync(new Uri(_burgerImageUrl));

            // Assert
            Assert.NotNull(result);
            
            // Output object detection results
            if (result.Objects != null)
            {
                _output.WriteLine("Detected Objects:");
                foreach (var obj in result.Objects.Values)
                {
                    if (obj.Tags.Count > 0)
                    {
                        _output.WriteLine($"- {obj.Tags.First().Name} (Bounding box: {obj.BoundingBox})");
                    }
                }
            }

            // Output dense captions
            if (result.DenseCaptions != null)
            {
                _output.WriteLine("Dense Captions:");
                foreach (var caption in result.DenseCaptions.Values)
                {
                    _output.WriteLine($"- {caption.Text} (Confidence: {caption.Confidence:P2})");
                }
            }

            // Verify the analysis found at least some objects or captions
            Assert.True(
                (result.Objects != null && result.Objects.Values.Count > 0) || 
                (result.DenseCaptions != null && result.DenseCaptions.Values.Count > 0),
                "Analysis should detect objects or provide dense captions for the burger image"
            );
        }
    }
}