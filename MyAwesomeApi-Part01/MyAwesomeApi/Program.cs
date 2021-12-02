var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/hello/{name}", (string name) => $"Hello {name.ToUpper()}! Nice to meet you.");

app.Run();