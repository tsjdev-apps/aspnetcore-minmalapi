using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MyAwesomeApi.Database;
using MyAwesomeApi.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace MyAwesomeApi.Tests
{
    public class TodoApiTests
    {
        [Fact]
        public async Task GetTodoItemsAsync()
        {
            await using var application = new TodoApplication();

            var client = application.CreateClient();
            var todos = await client.GetFromJsonAsync<List<TodoItem>>("/todoitems");

            Assert.Empty(todos);
        }

        [Fact]
        public async Task PostTodoItemAsync()
        {
            await using var application = new TodoApplication();

            var client = application.CreateClient();
            var response = await client.PostAsJsonAsync("/todoitems", new TodoItem { Content = "My Todo Item" });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var todos = await client.GetFromJsonAsync<List<TodoItem>>("/todoitems");

            var todoItem = Assert.Single(todos);
            Assert.Equal("My Todo Item", todoItem.Content);
            Assert.False(todoItem.IsCompleted);
        }

        [Fact]
        public async Task UpdateTodoItemAsync()
        {
            await using var application = new TodoApplication();

            var client = application.CreateClient();
            var response = await client.PostAsJsonAsync("/todoitems", new TodoItem { Content = "My Todo Item" });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var todos = await client.GetFromJsonAsync<List<TodoItem>>("/todoitems");

            var todoItem = Assert.Single(todos);
            Assert.Equal("My Todo Item", todoItem.Content);
            Assert.False(todoItem.IsCompleted);

            response = await client.PutAsJsonAsync($"/todoitems/{todoItem.Id}", new TodoItem { Id = todoItem.Id, Content = "My Updated Todo Item", IsCompleted = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            todos = await client.GetFromJsonAsync<List<TodoItem>>("/todoitems");
            todoItem = Assert.Single(todos);

            Assert.Equal("My Updated Todo Item", todoItem.Content);
            Assert.True(todoItem.IsCompleted);
        }

        [Fact]
        public async Task DeleteTodoItemAsync()
        {
            await using var application = new TodoApplication();

            var client = application.CreateClient();
            var response = await client.PostAsJsonAsync("/todoitems", new TodoItem { Content = "My Todo Item" });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var todos = await client.GetFromJsonAsync<List<TodoItem>>("/todoitems");

            var todoItem = Assert.Single(todos);
            Assert.Equal("My Todo Item", todoItem.Content);
            Assert.False(todoItem.IsCompleted);

            response = await client.DeleteAsync($"/todoitems/{todoItem.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.GetAsync($"/todoitems/{todoItem.Id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    class TodoApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<TodoDbContext>));

                services.AddDbContext<TodoDbContext>(options =>
                {
                    options.UseInMemoryDatabase("Testing", root);
                });
            });

            return base.CreateHost(builder);
        }
    }
}