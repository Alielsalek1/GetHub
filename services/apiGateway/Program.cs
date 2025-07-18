using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Load proxy configuration from appsettings.json
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Map reverse proxy to handle requests
app.MapReverseProxy();

app.Run();
