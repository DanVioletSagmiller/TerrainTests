using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Paint By Height")]
public class HeightPainterTerrainModLayer : TerrainModLayer
{
    public float[,,] Splat = null;
    public bool Change = false;
    public int SquareSize = 1;

    public void OnValidate()
    {
        if (Change) Change = false;
        Tool.OnValidate();
    }

    public override void Rebuild()
    {
        Splat = new float[
            Tool._TerrainData.alphamapWidth, 
            Tool._TerrainData.alphamapHeight, 
            Tool._TerrainData.alphamapLayers];

        for (int x = 0; x < Tool._TerrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < Tool._TerrainData.alphamapWidth; y++)
            {
                for(int l = 0; l < Tool._TerrainData.alphamapLayers; l++)
                {
                    var xOn = x % SquareSize + 1 > SquareSize / 2;
                    var yOn = y % SquareSize + 1 > SquareSize / 2;
                    if (xOn) yOn = !yOn;
                    if (yOn) Splat[x, y, l] = 1;
                    else Splat[x, y, l] = 0;
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
