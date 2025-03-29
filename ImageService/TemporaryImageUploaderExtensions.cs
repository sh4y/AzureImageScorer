using Microsoft.Extensions.DependencyInjection;

namespace ImageService.ImageAnalysis.Helpers;

/// <summary>
/// Extensions for registering the temporary image uploader
/// </summary>
public static class TemporaryImageUploaderExtensions
{
    /// <summary>
    /// Adds the temporary image uploader to the service collection
    /// </summary>
    public static IServiceCollection AddTemporaryImageUploader(this IServiceCollection services)
    {
        services.AddScoped<TemporaryImageUploader>();
        return services;
    }
}