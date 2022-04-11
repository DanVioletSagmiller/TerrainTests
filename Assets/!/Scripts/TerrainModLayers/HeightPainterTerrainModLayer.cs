using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
#endif // UNITY_EDITOR

[CreateAssetMenu(menuName = "Terrain/Layers/Paint By Height")]
public class HeightPainterTerrainModLayer : TerrainModLayer
{
    [System.Serializable]
    public class TopologyLayer
    {
        public int TargetHeight = 0;
        public int Index = 0;
    }

    public float[,,] Splat = null;
    public List<TopologyLayer> Topologies;

    public void OnValidate()
    {
        Tool.OnValidate();
    }

    public override void Rebuild()
    {
        var maxX = Tool._TerrainData.alphamapWidth;
        var maxY = Tool._TerrainData.alphamapHeight;
        var layerCount = Tool._TerrainData.alphamapLayers;
        Splat = new float[maxX, maxY, layerCount];

        Topologies.Sort((a, b) => a.TargetHeight - b.TargetHeight);
        bool usedTopology = false;
        for (int x = 0; x < Tool._TerrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < Tool._TerrainData.alphamapHeight; y++)
            {
                float percentX = (float)x / (float)maxX;
                float percentY = (float)y / (float)maxY;
                float height = Tool._TerrainData.GetHeight(
                    Mathf.RoundToInt(percentY * Tool.HeightRes), 
                    Mathf.RoundToInt(percentX * Tool.HeightRes));

                usedTopology = false;
                for (int i = 1; i < Topologies.Count; i++)
                {
                    if(Topologies[i].TargetHeight < height)
                    {
                        usedTopology = true;
                        Splat[x, y, Topologies[i - 1].Index] = 1;
                        break;
                    }
                }

                if (!usedTopology)
                {
                    Splat[x, y, Topologies[Topologies.Count - 1].Index] = 1;
                }
            }
        }
    }

    public override void Apply()
    {
        if (Splat == null) Rebuild();
        Tool._TerrainData.SetAlphamaps(0, 0, Splat);
    }
}
