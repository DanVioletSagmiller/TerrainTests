using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Perlin")]
public class PerlinTerrainModLayer : TerrainModLayer
{
    [Range(0.0001f, 0.05f)]
    public float PerlinStretch;
    [Range(0f, 1f)]
    public float HeightMultiplier;
    public AnimationCurve PerlinSlope;

    [Space]
    [Range(0.0001f, 0.05f)]
    public float PerlinStretch2;
    [Range(0f, 1f)]
    public float HeightMultiplier2;
    public AnimationCurve PerlinSlope2;

    [Space]
    [Range(0.0001f, 0.05f)]
    public float PerlinFluxStretch;
    public AnimationCurve PerlinFluxSlope;

    private float[,] Mesh;

    public void OnValidate()
    {
        Tool.OnValidate();
    }

    public override void Rebuild()
    {
        Mesh = new float[Tool.HeightRes, Tool.HeightRes];

        for (int x = 0; x < Tool.HeightRes; x++)
        {
            for (int y = 0; y < Tool.HeightRes; y++)
            {
                Vector3 v = Tool.WorldPositionFromHeightMapIndex(x, y);

                var flux0 = PerlinFluxSlope.Evaluate(
                    Mathf.PerlinNoise(
                        v.x * PerlinFluxStretch,
                        v.z * PerlinFluxStretch));

                var flux1 = 1f - flux0;

                var perlin1 = PerlinSlope.Evaluate(
                        Mathf.PerlinNoise(
                            v.x * PerlinStretch,
                            v.z * PerlinStretch))
                    * HeightMultiplier;

                var perlin2 = PerlinSlope2.Evaluate(
                        Mathf.PerlinNoise(
                            v.x * PerlinStretch2,
                            v.z * PerlinStretch2))
                    * HeightMultiplier2;

                Mesh[x, y] =
                    (perlin1 * flux0 + perlin2 * flux1);
            }
        }
    }

    public override void Apply()
    {
        if (Mesh == null) Rebuild();

        for (int x = 0; x < Tool.HeightRes; x++)
        {
            for (int y = 0; y < Tool.HeightRes; y++)
            {
                Tool.Mesh[x, y] = Mesh[x, y];
            }
        }
    }
}
