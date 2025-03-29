using System;
using System.Linq;

namespace ImageAnalysis
{
    /// <summary>
    /// Example usage of the image analyzer
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // Replace with your actual endpoint and key
            string endpoint = "YOUR_ENDPOINT";
            string key = "YOUR_KEY";
            Uri imageURL = new Uri("https://loveincorporated.blob.core.windows.net/contentimages/gallery/5366115e-decc-4024-941a-5237627bfa21-world-foods-tacos-shutterstock.jpg");

            // Perform image analysis in two lines
            var analyzer = new ImageAnalyzer(endpoint, key);
            var result = analyzer.Analyze(imageURL);
            
            // Optionally print the results
            ImageAnalysisResultProcessor.PrintResults(result);
        }
    }
}