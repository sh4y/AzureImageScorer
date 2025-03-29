using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
// Requires System.Net.Http.Json NuGet package

// For HttpUtility

// Define a namespace for your client library
namespace ImageShared
{
    public class ImageServiceClient : IDisposable
    {
        // HttpClient is intended to be instantiated once and re-used throughout the life of an application.
        // Injections or factories are preferred in larger applications, but static is fine for this example.
        private static readonly HttpClient client = new HttpClient();
        private readonly string _baseUrl;

        // Constructor allowing the base URL to be set
        public ImageServiceClient(string serviceBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceBaseUrl))
            {
                throw new ArgumentException("Service base URL cannot be empty.", nameof(serviceBaseUrl));
            }
            _baseUrl = serviceBaseUrl.TrimEnd('/'); // Ensure no trailing slash
        }

        // Calls the GET /api/ImageAnalysis/analyze endpoint
        public async Task AnalyzeImageFromUrlAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                Console.WriteLine("Image URL cannot be empty.");
                return;
            }

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["imageUrl"] = imageUrl;
            string requestUrl = $"{_baseUrl}/api/ImageAnalysis/analyze?{queryString}";

            Console.WriteLine($"Sending GET request to: {requestUrl}");

            try
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                await ProcessResponseAsync(response);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                if (e.InnerException != null) Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
                Console.WriteLine($"\nIs your ImageService running at {_baseUrl}?");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        // Calls the POST /api/ImageAnalysis/upload endpoint
        public async Task UploadAndAnalyzeImageAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            string requestUrl = $"{_baseUrl}/api/ImageAnalysis/upload";
            Console.WriteLine($"Sending POST request to: {requestUrl}");

            try
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(filePath);
                using var streamContent = new StreamContent(fileStream);
                using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());

                string contentType = Path.GetExtension(filePath).ToLowerInvariant() switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    _ => "application/octet-stream"
                };
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

                form.Add(fileContent, "file", Path.GetFileName(filePath)); // "file" matches the controller parameter name

                HttpResponseMessage response = await client.PostAsync(requestUrl, form);
                await ProcessResponseAsync(response);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                if (e.InnerException != null) Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
                Console.WriteLine($"\nIs your ImageService running at {_baseUrl}?");
            }
             catch (IOException ex)
            {
                 Console.WriteLine($"File error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        // Helper to process the HTTP response
        private async Task ProcessResponseAsync(HttpResponseMessage response)
        {
             if (response.IsSuccessStatusCode)
            {
                 string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Request successful. Raw JSON response:");
                 // Pretty print JSON
                try {
                   using var jDoc = JsonDocument.Parse(jsonResponse);
                   Console.WriteLine(JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true }));
                } catch (JsonException ex) {
                   Console.WriteLine($"Error parsing JSON: {ex.Message}");
                   Console.WriteLine(jsonResponse); // Print raw string if JSON parsing fails
                }
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine($"Error details: {errorContent}");
            }
        }

        // Optional: Implement IDisposable if HttpClient needs disposal (though typically not with static)
        public void Dispose()
        {
            // If not static, dispose HttpClient here: client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}