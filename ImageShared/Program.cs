

// Use the namespace defined for your client

namespace ImageShared;

public class Program
{
    // *** Replace with the actual URL where your ImageService is running ***
    private const string ServiceBaseUrl = "https://localhost:5001"; // Example: Use https if configured

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Image Analysis Service Client Runner");
        Console.WriteLine("----------------------------------");

        // Create an instance of the client
        using var client = new ImageServiceClient(ServiceBaseUrl);

        // --- Option 1: Analyze image by URL ---
        string imageUrlToAnalyze = "https://www.citypng.com/public/uploads/preview/chicken-burger-with-flying-ingredients-hd-transparent-png-701751710853243v9zgwqwepn.png?v=2025032803"; // Example image URL
        Console.WriteLine($"\nAnalyzing image from URL: {imageUrlToAnalyze}");
        await client.AnalyzeImageFromUrlAsync(imageUrlToAnalyze);

        Console.WriteLine("\n-----------------------------\n");

        // --- Option 2: Upload local image for analysis ---
        // *** Replace with the actual path to an image file on your PC ***
        string localImagePath = @"C:\path\to\your\image.jpg";
        Console.WriteLine($"Uploading and analyzing local image: {localImagePath}");
        await client.UploadAndAnalyzeImageAsync(localImagePath);

        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
    }
}