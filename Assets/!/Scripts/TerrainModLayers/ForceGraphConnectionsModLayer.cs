using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Force Graph Connections Layer")]
public class ForceGraphConnectionsModLayer : TerrainModLayer
{
    private float[,] Mesh = null;

    public void OnValidate()
    {
        Tool.OnValidate();
    }

    public override void Rebuild() 
    {
        var forceGraph = Tool.GetLayer<ForceGraphTerrainModLayer>();
        if (forceGraph == null)
        {
            Debug.LogError("TerrainTools is currently setup with ForceGraphConnectionsModLayer, but that requires ForceGraphTerrainModLayer is also available.");
        }

        var nodes = forceGraph.ForceGraph.Nodes;

        for (int a = 0; a < nodes.Count - 1; a++)
        {
            if (nodes[a].ConnectedNodes.Count == 0)
            {
                continue;
            }

            var na = nodes[a];
            for (int b = 0; b < na.ConnectedNodes.Count; b++)
            {

            }
        }
    }

    public override void Apply()
    {
        if (Mesh == null)
        {
            Rebuild();
        }



    }
}
