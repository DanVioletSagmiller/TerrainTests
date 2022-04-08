using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Paint By Height")]
public class HeightPainterTerrainModLayer : TerrainModLayer
{
    public float[,,] Splat = null;
    public int SquareSize = 1;

    public void OnValidate()
    {
        Tool.OnValidate();
    }

    public override void Rebuild()
    {
        Splat = new float[
            Tool._TerrainData.alphamapWidth, 
            Tool._TerrainData.alphamapHeight, 
            Tool._TerrainData.alphamapLayers];

        var doubleSize = SquareSize * 2;

        for (int x = 0; x < Tool._TerrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < Tool._TerrainData.alphamapHeight; y++)
            {
                var xOn = (x % doubleSize) + 1 > SquareSize;
                var yOn = (y % doubleSize) + 1 > SquareSize;
                //if (xOn) yOn = !yOn;
                if (yOn && !xOn || !yOn && xOn) Splat[x, y, 0] = 0;
                else Splat[x, y, 1] = 1;
            }
        }
    }

    public override void Apply()
    {
        if (Splat == null) Rebuild();
        Tool._TerrainData.SetAlphamaps(0, 0, Splat);
    }
}
