using Azure.AI.Vision.ImageAnalysis;
using ImageAnalysis;
using Microsoft.Extensions.Logging;

namespace ImageService;

/// <summary>
/// Middleware service for image analysis
/// </summary>
public class ImageAnalysisService : IImageAnalysisService
{
    private readonly IImageAnalyzer _analyzer;
    private readonly ILogger<ImageAnalysisService> _logger;

    /// <summary>
    /// Creates a new instance of the image analysis service
    /// </summary>
    public ImageAnalysisService(IImageAnalyzer analyzer, ILogger<ImageAnalysisService> logger)
    {
        _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public ImageAnalysisResult AnalyzeImage(Uri imageUrl)
    {
        if (imageUrl == null)
            throw new ArgumentNullException(nameof(imageUrl));

        _logger.LogInformation("Analyzing image at {Url}", imageUrl);
            
        try
        {
            var result = _analyzer.Analyze(imageUrl);
            _logger.LogInformation("Successfully analyzed image");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing image at {Url}", imageUrl);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ImageAnalysisResult> AnalyzeImageAsync(Uri imageUrl)
    {
        if (imageUrl == null)
            throw new ArgumentNullException(nameof(imageUrl));

        _logger.LogInformation("Asynchronously analyzing image at {Url}", imageUrl);
            
        // For this example, we're simulating async behavior
        // In a real implementation, the analyzer would have a proper AnalyzeAsync method
        return await Task.Run(() => AnalyzeImage(imageUrl));
    }
}