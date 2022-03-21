using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Island")]
public class IslandTerrainModLayer : TerrainModLayer
{
    public AnimationCurve EdgeBorder;

    [Range(0f, 1f)]
    public float PercentDistanceFromEdge;

    private float[,] EdgeReductionMap;

    public void OnValidate()
    {
        Tool.OnValidate();
    }

    public override void Rebuild()
    {
        EdgeReductionMap = new float[Tool.HeightRes, Tool.HeightRes];

        int half = Tool.HeightRes / 2;
        var center = new Vector2Int(half, half);
        var highBorder = PercentDistanceFromEdge * half;
        var range = half - highBorder;
        for (int x = 0; x < Tool.HeightRes; x++)
        {
            for (int y = 0; y < Tool.HeightRes; y++)
            {
                var distance = Vector2Int.Distance(center, new Vector2Int(x, y)) - highBorder;

                if (distance < 0)
                {
                    EdgeReductionMap[x, y] = 1;
                    continue;
                }

                if (distance > range)
                {
                    EdgeReductionMap[x, y] = 0;
                    continue;
                }

                EdgeReductionMap[x, y] = EdgeBorder.Evaluate(1 - distance / range);
            }
        }
    }

    public override void Apply()
    {
        if (EdgeReductionMap == null) Rebuild();

        for(int x = 0; x < Tool.HeightRes; x++)
        {
            for(int y = 0; y < Tool.HeightRes; y++)
            {
                Tool.Mesh[x, y] *= EdgeReductionMap[x, y];
            }
        }
    }
}
