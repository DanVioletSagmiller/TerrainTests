using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// <remarks>FullExecution() with FrameTime being an average frametime
///  - or - 
/// SingleStepExecution() with FrameTime being Time.deltaTime</remarks>
public class ForceGraph
{
    public class Node
    {
        public List<Node> ConnectedNodes = new List<Node>();
        public Object ExternalDetails;
        public Vector3 Position;
    }

    public List<Node> Nodes = new List<Node>();
    public ForceGraphSettings Settings;
    public Vector3 AnchorPosition = Vector3.zero;

    public void FullExecution()
    {
        for(int i = 0; i < Settings.Iterations; i++)
        {
            SingleStepExecution();
        }
    }

    public void SingleStepExecution()
    {
        foreach (var n1 in Nodes)
        {
            ApplyPull(n1, AnchorPosition, n1.Position.magnitude);
            foreach (var n2 in Nodes)
            {
                if (n1 == n2) continue;

                var connected = n1.ConnectedNodes.Contains(n2);
                var distance = (n1.Position - n2.Position).magnitude;

                if (connected)
                {
                    ApplyPull(n1, n2.Position, distance);

                }

                ApplyPush(n1, n2.Position, distance);

            }
        }
    }

    private void ApplyPush(Node n1, Vector3 toPosition, float distance)
    {
        var diff = n1.Position - toPosition;
        var dir = diff.normalized;
        var force =
            Settings.MoveForce
            * dir
            * (1f - (Mathf.Clamp(distance, 0, 1f)));

        n1.Position += force;
    }

    private void ApplyPull(Node n1, Vector3 toPosition, float distance)
    {
        var diff = n1.Position - toPosition;
        var dir = diff.normalized;
        var force =
            Settings.MoveForce
            * dir
            * Mathf.Clamp(distance, 0, 1f);

        n1.Position -= force;
    }
}
