using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo
{
    Version = "v1",
    Title = "My Minimal API",
    Description = "An sample ASP.NET Core Web API for testing the minimal API introduced with .NET6",
    Contact = new OpenApiContact
    {
        Name = "tsjdev-apps.de",
        Url = new Uri("https://www.tsjdev-apps.de")
    },
    License = new OpenApiLicense
    {
        Name = "MIT License",
        Url = new Uri("https://opensource.org/licenses/MIT")
    }
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/hello/{name}", (string name) => $"Hello {name.ToUpper()}! Nice to meet you.");

app.Run();