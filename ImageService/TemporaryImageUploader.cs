using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ImageService.ImageAnalysis.Helpers;

/// <summary>
/// Helper for uploading images to temporary blob storage
/// </summary>
public class TemporaryImageUploader
{
    private readonly string _connectionString;
    private readonly string _containerName;
    private readonly ILogger<TemporaryImageUploader> _logger;
    private readonly TimeSpan _expiryTime = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Creates a new instance of the temporary image uploader
    /// </summary>
    public TemporaryImageUploader(
        IConfiguration configuration,
        ILogger<TemporaryImageUploader> logger)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
        _connectionString = configuration["BlobStorage:ConnectionString"] 
                            ?? throw new ArgumentException("Missing BlobStorage:ConnectionString configuration");
                
        _containerName = configuration["BlobStorage:TempContainer"] 
                         ?? throw new ArgumentException("Missing BlobStorage:TempContainer configuration");
                
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Uploads an image from a file to temporary blob storage
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <returns>The URL to the uploaded blob</returns>
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (file.Length == 0) throw new ArgumentException("File is empty", nameof(file));

        try
        {
            // Create a unique blob name
            string blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                
            // Get a reference to the container
            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                
            // Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(blobName);
                
            // Set the content type
            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                }
            };
                
            // Upload the file
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, options);
            }
                
            // Set the expiration time
            await SetBlobExpiryAsync(blobClient);
                
            // Return the URL
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to temporary blob storage");
            throw;
        }
    }
        
    /// <summary>
    /// Uploads an image from a stream to temporary blob storage
    /// </summary>
    /// <param name="stream">The image stream to upload</param>
    /// <param name="contentType">The content type of the image</param>
    /// <param name="fileExtension">The file extension (include the dot)</param>
    /// <returns>The URL to the uploaded blob</returns>
    public async Task<string> UploadImageAsync(Stream stream, string contentType, string fileExtension)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (string.IsNullOrEmpty(contentType)) throw new ArgumentException("Content type is required", nameof(contentType));
        if (string.IsNullOrEmpty(fileExtension)) throw new ArgumentException("File extension is required", nameof(fileExtension));
            
        try
        {
            // Create a unique blob name
            string blobName = $"{Guid.NewGuid()}{fileExtension}";
                
            // Get a reference to the container
            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                
            // Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(blobName);
                
            // Set the content type
            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };
                
            // Upload the stream
            await blobClient.UploadAsync(stream, options);
                
            // Set the expiration time
            await SetBlobExpiryAsync(blobClient);
                
            // Return the URL
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to temporary blob storage");
            throw;
        }
    }
        
    /// <summary>
    /// Downloads an image from a URL and uploads it to temporary blob storage
    /// </summary>
    /// <param name="imageUrl">The URL of the image to download</param>
    /// <returns>The URL to the uploaded blob</returns>
    public async Task<string> UploadFromUrlAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) throw new ArgumentException("Image URL is required", nameof(imageUrl));
            
        try
        {
            // Download the image
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                using (var response = await httpClient.GetAsync(imageUrl))
                {
                    response.EnsureSuccessStatusCode();
                        
                    // Get the content type
                    var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                        
                    // Determine file extension from content type or URL
                    string fileExtension = Path.GetExtension(new Uri(imageUrl).AbsolutePath);
                    if (string.IsNullOrEmpty(fileExtension))
                    {
                        fileExtension = contentType switch
                        {
                            "image/jpeg" => ".jpg",
                            "image/png" => ".png",
                            "image/gif" => ".gif",
                            "image/bmp" => ".bmp",
                            _ => ".bin"
                        };
                    }
                        
                    // Get the stream
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        // Upload to blob storage
                        return await UploadImageAsync(stream, contentType, fileExtension);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image from URL to temporary blob storage");
            throw;
        }
    }
        
    /// <summary>
    /// Sets the expiry time on a blob
    /// </summary>
    private async Task SetBlobExpiryAsync(BlobClient blobClient)
    {
        // Set the expiration time
        var expiryTime = DateTimeOffset.UtcNow.Add(_expiryTime);
            
        // Create a lease on the blob
        var leaseClient = blobClient.GetBlobLeaseClient();
            
        // Update blob metadata with expiry time
        var blobProperties = await blobClient.GetPropertiesAsync();
        var metadata = blobProperties.Value.Metadata;
        metadata["ExpiryTime"] = expiryTime.ToString("O");
            
        // Update the metadata
        await blobClient.SetMetadataAsync(metadata);
            
        // Set an Azure-managed expiration policy
        await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders 
        {
            ContentType = blobProperties.Value.ContentType,
            CacheControl = $"max-age={_expiryTime.TotalSeconds}"
        });
    }
}