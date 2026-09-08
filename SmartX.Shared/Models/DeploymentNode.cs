namespace SmartX.Shared.Models;

public sealed class DeploymentNode
{
    public string Name { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;

    public Guid? SensorId { get; set; }

    public List<DeploymentNode> Children { get; set; } = [];
}