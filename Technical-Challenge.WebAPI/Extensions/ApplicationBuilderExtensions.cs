using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Technical_Challenge.Infrastructure.Persistence.Context;

namespace Technical_Challenge.WebAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication ConfigureWebApi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Quotes API v1"));

        app.UseCors();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var efContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await efContext.Database.MigrateAsync();
    }
}
