using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace AangTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        public string SecretKey { get; set; }
        public AppTestContext Context { get; set; }
        public ServiceProvider Provider { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppTestContext>));
                services.Remove(contextDescriptor);

                var connectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                services.Remove(connectionDescriptor);

                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<AppTestContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });

                Provider = services.BuildServiceProvider();
            });

            builder.UseEnvironment("Development");
        }

        public AppTestContext GetDBContext<AppTestContext>() where AppTestContext : DbContext
        {
            var scopeFactory = Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppTestContext>();

            return dbContext;
        }
    }
}
