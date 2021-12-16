using Microsoft.EntityFrameworkCore;
using MyAwesomeApi.Models;

namespace MyAwesomeApi.Database;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options) { }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}

