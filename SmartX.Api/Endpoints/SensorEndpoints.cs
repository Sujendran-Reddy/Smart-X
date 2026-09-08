using System.ComponentModel.DataAnnotations;
using SmartX.Api.Services;
using SmartX.Shared.Requests;

namespace SmartX.Api.Endpoints;

public static class SensorEndpoints
{
    public static void MapSensorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sensors");

        group.MapGet("/", (SensorRegistry registry) =>
        {
            return Results.Ok(registry.GetAll());
        });

        group.MapGet("/{id:guid}", IResult (
            Guid id,
            SensorRegistry registry) =>
        {
            var sensor = registry.GetById(id);

            if (sensor is null)
            {
                return Results.Problem(
                    title: "Sensor not found",
                    detail: "No registered sensor matches the supplied ID.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            return Results.Ok(sensor);
        });

        group.MapPost("/", IResult (
            RegisterSensorRequest request,
            SensorRegistry registry) =>
        {
            request.DeviceIdentifier =
                request.DeviceIdentifier?.Trim() ?? string.Empty;

            request.DeploymentLocation =
                request.DeploymentLocation?.Trim() ?? string.Empty;

            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(
                request,
                new ValidationContext(request),
                validationResults,
                validateAllProperties: true);

            if (!isValid)
            {
                var errors = validationResults
                    .SelectMany(result => result.MemberNames
                        .DefaultIfEmpty(string.Empty)
                        .Select(member => new
                        {
                            Member = member,
                            Message = result.ErrorMessage ?? "Invalid value."
                        }))
                    .GroupBy(error => error.Member)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.Message).ToArray());

                return Results.ValidationProblem(errors);
            }

            var sensor = registry.Register(request);

            if (sensor is null)
            {
                return Results.Problem(
                    title: "Device already registered",
                    detail: "A sensor with this device identifier already exists.",
                    statusCode: StatusCodes.Status409Conflict);
            }

            return Results.Created($"/api/sensors/{sensor.Id}", sensor);
        });
    }
}