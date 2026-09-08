using SmartX.Api.Services;
using SmartX.Shared.Models;

namespace SmartX.Api.Endpoints;

public static class DeploymentEndpoints
{
    public static void MapDeploymentEndpoints(this WebApplication app)
    {
        app.MapPost(
            "/api/deployments/validate",
            IResult (
                DeploymentNode root,
                DeploymentValidator validator) =>
            {
                var result = validator.Validate(root);

                if (!result.IsValid)
                {
                    return Results.BadRequest(result);
                }

                return Results.Ok(result);
            });
    }
}