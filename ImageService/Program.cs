using ImageService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

// For .NET 9.0 we use the minimal hosting model
var builder = WebApplication.CreateBuilder(args);

// Add services to the container using the Startup class
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline using the Startup class
startup.Configure(app, app.Environment);

app.Run();