using SmartX.Shared.Models;
using SmartX.Api.Endpoints;
using SmartX.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<SensorRegistry>();

builder.Services.AddSingleton<DeploymentValidator>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.MaxDepth = 256;
});

builder.Services.AddSingleton<SensorAttachmentStore>();

builder.Services.AddSingleton<TelemetryStore<float>>();

builder.Services.AddSingleton<TelemetryStore<int>>();

builder.Services.AddSingleton<TelemetryStore<bool>>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Dashboard", policy =>
    {
        policy.WithOrigins("https://localhost:7102")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Dashboard");

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        application = "SmartX.Api"
    });
});

app.MapGet(
    "/api/power/aggregate",
    IResult (int firstWatts, int secondWatts, int limitWatts) =>
    {
        if (firstWatts < 0 || secondWatts < 0 || limitWatts <= 0)
        {
            return Results.Problem(
                title: "Invalid power values",
                detail: "Readings must be zero or greater and the limit must be greater than zero.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var firstMeter = new PowerReading(firstWatts);
        var secondMeter = new PowerReading(secondWatts);
        var limit = new PowerReading(limitWatts);

        PowerReading total;

        try
        {
            total = firstMeter + secondMeter;
        }
        catch (OverflowException)
        {
            return Results.Problem(
                title: "Power total exceeds the supported range",
                detail: "The combined reading must not exceed 2,147,483,647 watts.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Results.Ok(new
        {
            firstWatts = firstMeter.Watts,
            secondWatts = secondMeter.Watts,
            totalWatts = total.Watts,
            limitWatts = limit.Watts,
            isOverLimit = total > limit
        });
    });

app.MapSensorEndpoints();

app.MapTelemetryEndpoints();

app.MapSensorAttachmentEndpoints();

app.MapDeploymentEndpoints();

app.Run();