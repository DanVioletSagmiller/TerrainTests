using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Force Graph")]
public class ForceGraphTerrainModLayer : TerrainModLayer
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
    public ForceGraph ForceGraph = new ForceGraph();

    public float Radius = 50f;

    private float[,] Mesh;
    private float[,] MeshStrength;

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
        if (IterationsRemaining < 0) OnValidate();

    }

    private void OnEnable()
    {
        if (Application.isPlaying) return;
        UnityEditor.EditorApplication.update += EditorUpdate;
    }


    public void OnValidate()
    {
        ForceGraph.Settings = Settings;

        if (_Stop)
        {
            _Stop = false;
            IterationsRemaining = -1;
        }

        Tool.OnValidate();
    }

    public override void Rebuild()
    {

        Mesh = new float[Tool.HeightRes, Tool.HeightRes];
        MeshStrength = new float[Tool.HeightRes, Tool.HeightRes];

        var anchor = ForceGraphVisualizerManager.Instance.transform.position;

        var height = Tool.GetTerrainPercentForWorldHeight(anchor.y);
        var res = Tool.HeightRes;
        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                foreach (var node in ForceGraph.Nodes)
                {
                    var pos = node.Position * Settings.Scaling + anchor;

                    var distance = ZeroDistance(
                        pos,
                        Tool.WorldPositionFromHeightMapIndex(x, y));

                    if (distance < Radius)
                    {
                        Mesh[x, y] = height;
                        MeshStrength[x, y] = 1f;
                        continue;
                    }
                }
            }
        }
    }

    private float ZeroDistance(Vector3 v1, Vector3 v2)
    {
        v1.y = 0;
        v2.y = 0;
        return Vector3.Distance(v1, v2);
    }

    public override void Apply()
    {
        if (Mesh == null) Rebuild();

        for(int x = 0; x < Tool.HeightRes; x++)
        {
            for(int y = 0; y < Tool.HeightRes; y++)
            {
                var overlayStrength = MeshStrength[x, y];
                var originalStrength = 1f - overlayStrength;
                Tool.Mesh[x, y] = 
                    (originalStrength * Tool.Mesh[x,y]) 
                    + (overlayStrength * Mesh[x, y]);
            }
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
}
