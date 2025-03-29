using Azure.AI.Vision.ImageAnalysis;

namespace ImageScorer;

/// <summary>
/// Result processor for image analysis
/// </summary>
public static class ImageAnalysisResultProcessor
{
    /// <summary>
    /// Prints the analysis results to the console
    /// </summary>
    /// <param name="result">Analysis result to print</param>
    public static void PrintResults(ImageAnalysisResult result)
    {
        Console.WriteLine("Image analysis results:");

        // Print caption results to the console
        Console.WriteLine(" Caption:");
        if (result.Caption != null)
        {
            Console.WriteLine($"   '{result.Caption.Text}', Confidence {result.Caption.Confidence:F4}");
        }

        // Print dense caption results to the console
        Console.WriteLine(" Dense Captions:");
        if (result.DenseCaptions != null)
        {
            foreach (DenseCaption denseCaption in result.DenseCaptions.Values)
            {
                Console.WriteLine($"   '{denseCaption.Text}', Confidence {denseCaption.Confidence:F4}, Bounding box {denseCaption.BoundingBox}");
            }
        }

        // Print object detection results to the console
        Console.WriteLine(" Objects:");
        if (result.Objects != null)
        {
            foreach (DetectedObject detectedObject in result.Objects.Values)
            {
                Console.WriteLine($"   '{detectedObject.Tags.First().Name}', Bounding box {detectedObject.BoundingBox}");
            }
        }

        // Print text (OCR) analysis results to the console
        Console.WriteLine(" Read:");
        if (result.Read != null)
        {
            foreach (DetectedTextBlock block in result.Read.Blocks)
            foreach (DetectedTextLine line in block.Lines)
            {
                Console.WriteLine($"   Line: '{line.Text}', Bounding Polygon: [{string.Join(" ", line.BoundingPolygon)}]");
                foreach (DetectedTextWord word in line.Words)
                {
                    Console.WriteLine($"     Word: '{word.Text}', Confidence {word.Confidence.ToString("#.####")}, Bounding Polygon: [{string.Join(" ", word.BoundingPolygon)}]");
                }
            }
        }

        // Print tags results to the console
        Console.WriteLine(" Tags:");
        if (result.Tags != null)
        {
            foreach (DetectedTag tag in result.Tags.Values)
            {
                Console.WriteLine($"   '{tag.Name}', Confidence {tag.Confidence:F4}");
            }
        }

        // Print people detection results to the console
        Console.WriteLine(" People:");
        if (result.People != null)
        {
            foreach (DetectedPerson person in result.People.Values)
            {
                Console.WriteLine($"   Person: Bounding box {person.BoundingBox}, Confidence {person.Confidence:F4}");
            }
        }

        // Print smart-crops analysis results to the console
        Console.WriteLine(" SmartCrops:");
        if (result.SmartCrops != null)
        {
            foreach (CropRegion cropRegion in result.SmartCrops.Values)
            {
                Console.WriteLine($"   Aspect ratio: {cropRegion.AspectRatio}, Bounding box: {cropRegion.BoundingBox}");
            }
        }

        // Print metadata
        Console.WriteLine(" Metadata:");
        if (result.Metadata != null)
        {
            Console.WriteLine($"   Model: {result.ModelVersion}");
            Console.WriteLine($"   Image width: {result.Metadata.Width}");
            Console.WriteLine($"   Image height: {result.Metadata.Height}");
        }
    }
}