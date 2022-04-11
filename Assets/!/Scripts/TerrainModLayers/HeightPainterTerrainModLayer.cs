using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Paint By Height")]
public class HeightPainterTerrainModLayer : TerrainModLayer
{

    public float[,,] Splat = null;
    public float[] Heights;

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

        bool usedTopology = false;
        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                float percentX = (float)x / (float)maxX;
                float percentY = (float)y / (float)maxY;
                float height = Tool.transform.position.y 
                    + Tool._TerrainData.heightmapScale.y 
                    * Tool.Mesh[
                        Mathf.RoundToInt(percentX * Tool.HeightRes), 
                        Mathf.RoundToInt(percentY * Tool.HeightRes)];

                usedTopology = false;
                for (int i = 1; i < Heights.Length; i++)
                {
                    if(Heights[i] > height)
                    {
                        usedTopology = true;
                        Splat[x, y, i - 1] = 1;
                        break;
                    }
                }

                if (!usedTopology)
                {
                    Splat[x, y, Heights.Length - 1] = 1;
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
