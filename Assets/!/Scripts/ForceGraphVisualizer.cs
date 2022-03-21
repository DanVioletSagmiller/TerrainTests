using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ForceGraphVisualizer : MonoBehaviour
{
    [Header("Visual Configuration")]
    public bool _InstantiateRandomNodes = false;
    public bool _Stop = false;
    public bool _Instant = true;

    [Range(0f, 1f)]
    public float PercentConnectionChance = 0.8f;
    public int InstantiationCount = 10;
    public ForceGraphSettings Settings;

    [Header("Debug")]
    public float IterationTimeMS = 0;
    private int IterationsRemaining = 0;
    private ForceGraph ForceGraph = new ForceGraph();

    private void EditorUpdate()
    {
        if (_InstantiateRandomNodes)
        {
            _InstantiateRandomNodes = false;
            InstantiateRandomNodes();
        }

        if (IterationsRemaining < 0) return;
        var duration = new System.Diagnostics.Stopwatch();
        duration.Start();
        if (_Instant)
        {
            IterationsRemaining = -1;
            ForceGraph.FullExecution();
        }
        else
        {
            IterationsRemaining -= 1;
            ForceGraph.SingleStepExecution();
        }
        duration.Stop();
        this.IterationTimeMS = (float)duration.ElapsedMilliseconds;
    }

    private void OnEnable()
    {
        if (Application.isPlaying) return;
        UnityEditor.EditorApplication.update += EditorUpdate;
    }

    private void OnValidate()
    {
        ForceGraph.MoveForce = Settings.MoveForce;
        ForceGraph.IterationsToStop = Settings.Iterations;

        if (_Stop)
        {
            _Stop = false;
            IterationsRemaining = -1;
        }
    }

    private void InstantiateRandomNodes()
    {
        IterationsRemaining = Settings.Iterations;

        // clear old nodes
        ForceGraph.Nodes.Clear();

        // generate nodes
        for (int i = 0; i < InstantiationCount; i++)
        {
            Vector2 pos = Random.insideUnitCircle;
            var posConverted = new Vector3(pos.x, 0, pos.y);
            ForceGraph.Nodes.Add(new ForceGraph.Node()
            {
                Position = posConverted
            });
        }

        // assign random connections
        var distrobutionCap = PercentConnectionChance * InstantiationCount;
        for (int i = 0; i < InstantiationCount - 1; i++)
        {
            var nodeI = ForceGraph.Nodes[i];
            if (nodeI.ConnectedNodes.Count >= distrobutionCap) continue;

            for (int j = i + 1; j < InstantiationCount; j++)
            {
                var nodeJ = ForceGraph.Nodes[j];

                if (nodeI.ConnectedNodes.Contains(nodeJ)) continue;

                if (Random.Range(0f, 1f) <= PercentConnectionChance)
                {
                    nodeI.ConnectedNodes.Add(nodeJ);
                    nodeJ.ConnectedNodes.Add(nodeI);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        //if (Settings == null) return;
        var offset = transform.position;
        // Draw Center Mark
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(offset, .01f * Settings.Scaling);

        // Draw connecting lines
        Gizmos.color = Color.red;
        foreach (var node in ForceGraph.Nodes)
        {
            foreach (var n2 in node.ConnectedNodes)
            {
                Gizmos.DrawLine(
                    node.Position * Settings.Scaling + offset, 
                    n2.Position * Settings.Scaling + offset);
            }
        }

        // Draw node points
        Gizmos.color = Color.black;
        foreach (var node in ForceGraph.Nodes)
        {
            Gizmos.DrawSphere(
                node.Position * Settings.Scaling + offset, 
                .025f * Settings.Scaling);
        }
    }
}
