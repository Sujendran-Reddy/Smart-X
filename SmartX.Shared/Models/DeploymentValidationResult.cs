namespace SmartX.Shared.Models;

public sealed class DeploymentValidationResult
{
    public int NodesVisited { get; set; }

    public int MaxDepthReached { get; set; }

    public List<string> Errors { get; set; } = [];

    public bool IsValid => Errors.Count == 0;
}