using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ForceGraphVisualizerManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public ForceGraphTerrainModLayer TerrainModLayer;

    public static ForceGraphVisualizerManager Instance;

    private void OnDrawGizmos()
    {
        if (TerrainModLayer == null
            || TerrainModLayer.Settings == null
            || TerrainModLayer.ForceGraph == null)
        {
            return;
        }

        var offset = transform.position;
        // Draw Center Mark
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(offset, .01f * TerrainModLayer.Settings.Scaling);

        // Draw connecting lines
        Gizmos.color = Color.red;
        foreach (var node in TerrainModLayer.ForceGraph.Nodes)
        {
            foreach (var n2 in node.ConnectedNodes)
            {
                Gizmos.DrawLine(
                    node.Position * TerrainModLayer.Settings.Scaling + offset, 
                    n2.Position * TerrainModLayer.Settings.Scaling + offset);
            }
        }

        // Draw node points
        Gizmos.color = Color.black;
        foreach (var node in TerrainModLayer.ForceGraph.Nodes)
        {
            Gizmos.DrawSphere(
                node.Position * TerrainModLayer.Settings.Scaling + offset, 
                .025f * TerrainModLayer.Settings.Scaling);
        }
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        Instance = this;
    }
}
