using Azure.AI.Vision.ImageAnalysis;

/// <summary>
/// Configuration for the image analyzer
/// </summary>
public class ImageAnalyzerConfiguration
{
    /// <summary>
    /// Gets the analysis options
    /// </summary>
    public ImageAnalysisOptions Options { get; }

    /// <summary>
    /// Gets the visual features to analyze
    /// </summary>
    public VisualFeatures VisualFeatures { get; }

    /// <summary>
    /// Creates a new instance with default settings
    /// </summary>
    public ImageAnalyzerConfiguration()
    {
        Options = new ImageAnalysisOptions
        {
            GenderNeutralCaption = true,
            Language = "en",
            SmartCropsAspectRatios = new float[] { 0.9F, 1.33F }
        };

        VisualFeatures =
            VisualFeatures.Caption |
            VisualFeatures.DenseCaptions |
            VisualFeatures.Objects |
            VisualFeatures.Read |
            VisualFeatures.Tags |
            VisualFeatures.People |
            VisualFeatures.SmartCrops;
    }
}