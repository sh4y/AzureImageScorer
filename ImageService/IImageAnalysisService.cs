using Azure.AI.Vision.ImageAnalysis;

namespace ImageService;

/// <summary>
/// Service interface for image analysis
/// </summary>
public interface IImageAnalysisService
{
    /// <summary>
    /// Analyzes an image synchronously
    /// </summary>
    ImageAnalysisResult AnalyzeImage(Uri imageUrl);

    /// <summary>
    /// Analyzes an image asynchronously
    /// </summary>
    Task<ImageAnalysisResult> AnalyzeImageAsync(Uri imageUrl);
}