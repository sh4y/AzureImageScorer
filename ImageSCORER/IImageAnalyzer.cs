using Azure.AI.Vision.ImageAnalysis;

namespace ImageAnalysis;

/// <summary>
/// Interface for image analysis operations
/// </summary>
public interface IImageAnalyzer
{
    /// <summary>
    /// Analyzes an image from a URL
    /// </summary>
    /// <param name="imageUrl">The URL of the image to analyze</param>
    /// <returns>The analysis result</returns>
    ImageAnalysisResult Analyze(Uri imageUrl);
}