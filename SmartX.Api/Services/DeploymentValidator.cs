using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class DeploymentValidator
{
    // limit the tree depth and number of nodes checked
    private const int MaxDepth = 64;
    private const int MaxNodes = 10000;

    private readonly SensorRegistry registry;

    public DeploymentValidator(SensorRegistry registry)
    {
        this.registry = registry;
    }

    // checks for reused nodes & duplicate sensor assignments seperately
    public DeploymentValidationResult Validate(DeploymentNode root)
    {
        var result = new DeploymentValidationResult();
        var visitedNodes = new HashSet<DeploymentNode>();
        var assignedSensors = new HashSet<Guid>();

        ValidateNode(
            root,
            "Root",
            1,
            true,
            visitedNodes,
            assignedSensors,
            result);

        return result;
    }

    private void ValidateNode(
        DeploymentNode? node,
        string path,
        int depth,
        bool ancestorsEnabled,
        HashSet<DeploymentNode> visitedNodes,
        HashSet<Guid> assignedSensors,
        DeploymentValidationResult result)
    {
        result.NodesVisited++;
        result.MaxDepthReached = Math.Max(result.MaxDepthReached, depth);

        if (result.NodesVisited > MaxNodes)
        {
            result.Errors.Add($"Deployment trees cannot exceed {MaxNodes} nodes.");
            return;
        }

        if (depth > MaxDepth)
        {
            result.Errors.Add($"{path}: Deployment depth cannot exceed {MaxDepth} levels.");
            return;
        }

        if (node is null)
        {
            result.Errors.Add($"{path}: A deployment node cannot be null.");
            return;
        }

        if (!visitedNodes.Add(node))
        {
            result.Errors.Add($"{path}: A cycle or repeated node reference was detected.");
            return;
        }

        if (string.IsNullOrWhiteSpace(node.Name) || node.Name.Length > 100)
        {
            result.Errors.Add($"{path}: Enter a node name between 1 and 100 characters.");
        }

        if (node.Children is null)
        {
            result.Errors.Add($"{path}: Children must be an array, even when empty.");
            return;
        }

        // a sensor must be enabled along with all its parent nodes

        var effectivelyEnabled = ancestorsEnabled && node.IsEnabled;

        if (node.SensorId.HasValue)
        {
            var sensorId = node.SensorId.Value;

            if (registry.GetById(sensorId) is null)
            {
                result.Errors.Add($"{path}: Sensor {sensorId} is not registered.");
            }

            if (!assignedSensors.Add(sensorId))
            {
                result.Errors.Add($"{path}: Sensor {sensorId} appears more than once.");
            }

            if (!effectivelyEnabled)
            {
                result.Errors.Add($"{path}: The sensor or one of its parent deployment nodes is disabled.");
            }

            if (node.Children.Count > 0)
            {
                result.Errors.Add($"{path}: Sensor nodes cannot contain child nodes.");
            }
        }
        else if (node.Children.Count == 0)
        {
            result.Errors.Add($"{path}: A grouping node must contain at least one child.");
        }

        for (var index = 0; index < node.Children.Count; index++)
        {
            ValidateNode(
                node.Children[index],
                $"{path}.Children[{index}]",
                depth + 1,
                effectivelyEnabled,
                visitedNodes,
                assignedSensors,
                result);

            if (result.NodesVisited > MaxNodes)
            {
                return;
            }
        }
    }
}