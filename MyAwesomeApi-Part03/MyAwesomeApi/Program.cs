using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyAwesomeApi.Database;
using MyAwesomeApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoItems"));

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

app.MapGet("/todoitems", async (TodoDbContext db) =>
{
    return await db.TodoItems.ToListAsync();
});

app.MapGet("/todoitems/{id}", async (int id, TodoDbContext db) =>
{
    var todoItem = await db.TodoItems.FindAsync(id);
    if (todoItem == null)
        return Results.NotFound();

    return Results.Ok(todoItem);
});

app.MapPost("/todoitems", async (TodoItem todoItem, TodoDbContext db) =>
{
    await db.TodoItems.AddAsync(todoItem);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem todoItem, TodoDbContext db) =>
{
    if (id != todoItem.Id)
        return Results.BadRequest();

    if (!await db.TodoItems.AnyAsync(x => x.Id == id))
        return Results.NotFound();

    db.Update(todoItem);
    await db.SaveChangesAsync();

    return Results.Ok(todoItem);
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDbContext db) =>
{
    var todoItem = await db.TodoItems.FindAsync(id);
    if (todoItem is null)
        return Results.NotFound();

    db.TodoItems.Remove(todoItem);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();