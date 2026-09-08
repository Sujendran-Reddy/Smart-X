using SmartX.Api.Services;

namespace SmartX.Api.Endpoints;

public static class DemoEndpoints
{
    public static void MapDemoEndpoints(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        app.MapPost(
            "/api/demo/seed",
            (DemoDataSeeder seeder) =>
            {
                return Results.Ok(seeder.Seed());
            });
    }
}