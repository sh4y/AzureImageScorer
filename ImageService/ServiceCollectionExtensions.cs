using ImageAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace ImageService;

/// <summary>
/// Configuration extensions for the service collection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds image analysis services to the service collection
    /// </summary>
    public static IServiceCollection AddImageAnalysis(
        this IServiceCollection services, 
        string endpoint, 
        string key)
    {
        // Register the analyzer
        services.AddSingleton<IImageAnalyzer>(provider => 
            new ImageAnalyzer(endpoint, key));
            
        // Register the middleware service
        services.AddScoped<IImageAnalysisService, ImageAnalysisService>();
            
        return services;
    }
}