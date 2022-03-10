using UnityEngine;

// SEE https://youtu.be/6NT-OoEdWQE for an explanation 3ply perlin slopes.
// SEE https://youtu.be/Vif8cdd-GVE for an explanation of 1 perlin slope combo.
// SEE https://youtu.be/iRgh1EUxs6Y for an explanation of island borders.


[ExecuteAlways]
public class TerrainTool : MonoBehaviour
{
    public Terrain _Terrain;
    public TerrainData _TerrainData;

    [Header("Perlin Land Shaping")]
    [Range(0.0001f, 0.05f)]
    public float PerlinStretch;
    [Range(0.0001f, 0.05f)]
    public float PerlinStretch2;
    [Range(0.0001f, 0.05f)]
    public float PerlinFluxStretch;

    [Range(0f, 1f)]
    public float HeightMultiplier;
    [Range(0f, 1f)]
    public float HeightMultiplier2;

    public AnimationCurve PerlinSlope;
    public AnimationCurve PerlinSlope2;
    public AnimationCurve PerlinFluxSlope;

    [Header("Island Border")]
    [Range(0f, 1f)]
    public float PercentDistanceFromEdge;
    public AnimationCurve EdgeBorder;

    public TerrainFlattener[] Flatteners;

    public float[,] Mesh
    {
        get
        {
            return _Mesh;
        }
        set
        {
            Mesh = value;
        }
    }

    private int HeightRes;
    private float[,] _Mesh;
    private float[,] EdgeReductionMap;
    private float PreviousPercentDistanceFromEdge;
    private AnimationCurve PreviousEdgeBorder;
    private Vector3 size;
    private Vector3 scale;


    public void OnEnable()
    {
        PreviousPercentDistanceFromEdge = float.MinValue;
        PreviousEdgeBorder = null;
        PullTerrain();
        BuildEdgeReductionMap();
        
    }

    public void Start()
    {
        this.enabled = false;
    }

    public void Update()
    {
        if (transform.hasChanged)
        {
            OnValidate();
        }
    }

    public void PullTerrain()
    {
        HeightRes = _TerrainData.heightmapResolution;
        _Mesh = _TerrainData.GetHeights(
            0,
            0,
            HeightRes,
            HeightRes);
    }

    public void OnValidate()
    {
        RefreshTerrain();
    }

    public void RefreshTerrain()
    {
        PullTerrain();
        size = _TerrainData.size;
        scale = size / HeightRes;

        if (PreviousPercentDistanceFromEdge != PercentDistanceFromEdge)
        {
            PreviousPercentDistanceFromEdge = PercentDistanceFromEdge;
            BuildEdgeReductionMap();
        }

        if (!EdgeBorder.Equals(PreviousEdgeBorder))
        {
            PreviousEdgeBorder = new AnimationCurve(EdgeBorder.keys);
            BuildEdgeReductionMap();
        }

        RedrawTerrainMesh();
        HandleFlatteners();
        PushChanges();
    }

    private void HandleFlatteners()
    {
        foreach (var flattener in Flatteners)
        {
            flattener.RefreshContent();
        }
    }

    private void BuildEdgeReductionMap()
    {
        
        EdgeReductionMap = new float[HeightRes, HeightRes];

        int half = HeightRes / 2;
        var center = new Vector2Int(half, half);
        var highBorder = PercentDistanceFromEdge * half;
        var range = half - highBorder;
        for (int x = 0; x < HeightRes; x++)
        {
            for (int y = 0; y < HeightRes; y++)
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

    public void RedrawTerrainMesh()
    {
        for (int x = 0; x < HeightRes; x++)
        {
            for (int y = 0; y < HeightRes; y++)
            {
                Vector3 v = WorldPositionFromHeightMapIndex(x, y);

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

                _Mesh[x, y] =
                    (perlin1 * flux0 + perlin2 * flux1)
                    * EdgeReductionMap[x, y];
            }
        }
    }

    public void PushChanges()
    {
        _TerrainData.SetHeights(0, 0, _Mesh);
    }

    public void OnBeforeSerialize() { }

    public Vector3 WorldPositionFromHeightMapIndex(int x, int y)
    {
        return
            new Vector3(
                y * scale.z,
                size.y * _Mesh[x, y],
                x * scale.x)
            + _Terrain.transform.position;
    }

    public float GetTerrainPercentForWorldHeight(float y)
    {
        if (y < _Terrain.transform.position.y) return 0;
        if (y > _Terrain.transform.position.y + size.y) return 1;
        y += 0 - _Terrain.transform.position.y;
        return y / size.y;
        
    }
}
