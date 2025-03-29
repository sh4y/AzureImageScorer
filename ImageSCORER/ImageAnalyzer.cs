using Azure;
using Azure.AI.Vision.ImageAnalysis;

namespace ImageAnalysis;

/// <summary>
/// Analyzer for Azure Computer Vision images
/// </summary>
public class ImageAnalyzer : IImageAnalyzer
{
    private readonly ImageAnalysisClient _client;
    private readonly ImageAnalyzerConfiguration _configuration;

    /// <summary>
    /// Creates a new instance of the image analyzer
    /// </summary>
    /// <param name="endpoint">Azure service endpoint</param>
    /// <param name="key">Azure service key</param>
    public ImageAnalyzer(string endpoint, string key)
    {
        _client = new ImageAnalysisClient(
            new Uri(endpoint),
            new AzureKeyCredential(key));

        _configuration = new ImageAnalyzerConfiguration();
    }

    /// <inheritdoc/>
    public ImageAnalysisResult Analyze(Uri imageUrl)
    {
        return _client.Analyze(
            imageUrl,
            _configuration.VisualFeatures,
            _configuration.Options);
    }
}