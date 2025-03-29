using Azure;
using Azure.AI.Vision.ImageAnalysis;
using ImageAnalysis.Api;
using ImageAnalysis.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json.Serialization;
using ImageService;
using ImageService.ImageAnalysis.Helpers;

namespace ImageAnalysis
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers with JSON options
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Image Analysis API",
                    Description = "API for analyzing and processing images",
                    Version = "v1"
                });
            });

            // Configure image analysis services
            ConfigureImageAnalysisServices(services);

            // Configure temporary blob storage
            services.AddTemporaryImageUploader();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
=                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image Analysis API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // Helper method to configure image analysis services
        private void ConfigureImageAnalysisServices(IServiceCollection services)
        {
            // Get Azure Vision configuration
            var endpoint = Configuration["AzureVision:Endpoint"];
            var key = Configuration["AzureVision:Key"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException(
                    "Azure Vision API credentials are not configured. " +
                    "Please set AzureVision:Endpoint and AzureVision:Key in appsettings.json or environment variables.");
            }

            // Register the analyzer with default configuration
            services.AddSingleton<IImageAnalyzer>(provider =>
            {
                var client = new ImageAnalysisClient(
                    new Uri(endpoint),
                    new AzureKeyCredential(key));

                return new ImageAnalyzer(endpoint, key);
            });

            // Register the service
            services.AddScoped<IImageAnalysisService, ImageAnalysisService>();
        }
    }
}