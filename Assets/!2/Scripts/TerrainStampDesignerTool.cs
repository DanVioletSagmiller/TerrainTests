using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainStampDesignerTool : MonoBehaviour
{
    public Terrain terrain;
    private Texture2D stampTexture;

    private void Awake()
    {
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (!terrain) terrain = Terrain.activeTerrain;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer && renderer.sharedMaterial)
        {
            stampTexture = renderer.sharedMaterial.mainTexture as Texture2D;
            if (stampTexture && !stampTexture.isReadable)
                Debug.LogError("Enable Read/Write in texture import settings for: " + stampTexture.name);
        }
    }

    public void AdjustTerrain()
    {
        ValidateComponents();
        if (!terrain || !stampTexture) return;

        TerrainData terrainData = terrain.terrainData;
        Undo.RegisterCompleteObjectUndo(terrainData, "Terrain Stamp");

        int resolution = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, resolution, resolution);

        float targetHeight = GetTargetHeight(terrainData);

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector3 worldPos = HeightmapToWorldPosition(x, z, terrainData);
                if (IsWithinQuadBounds(worldPos, transform.position, transform.lossyScale))
                {
                    Vector2 uv = WorldToQuadUV(worldPos, transform.position, transform.lossyScale);
                    float blend = SampleStampTexture(uv);
                    heights[z, x] = Mathf.Lerp(heights[z, x], targetHeight, blend);
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    private float GetTargetHeight(TerrainData terrainData)
    {
        return transform.position.y / terrainData.size.y;
    }

    private Vector3 HeightmapToWorldPosition(int x, int z, TerrainData terrainData)
    {
        return terrain.transform.position + new Vector3(
            x * terrainData.size.x / (terrainData.heightmapResolution - 1),
            0,
            z * terrainData.size.z / (terrainData.heightmapResolution - 1)
        );
    }

    // Check if the world position is within the X/Z space of the quad
    private bool IsWithinQuadBounds(Vector3 worldPos, Vector3 quadPos, Vector3 quadScale)
    {
        return worldPos.x >= quadPos.x - quadScale.x / 2f &&
               worldPos.x <= quadPos.x + quadScale.x / 2f &&
               worldPos.z >= quadPos.z - quadScale.z / 2f &&
               worldPos.z <= quadPos.z + quadScale.z / 2f;
    }

    private Vector2 WorldToQuadUV(Vector3 worldPos, Vector3 quadPos, Vector3 quadScale)
    {
        return new Vector2(
            (worldPos.x - (quadPos.x - quadScale.x / 2f)) / quadScale.x,
            (worldPos.z - (quadPos.z - quadScale.z / 2f)) / quadScale.z
        );
    }

    private float SampleStampTexture(Vector2 uv)
    {
        Color pixel = stampTexture.GetPixelBilinear(uv.x, uv.y);
        return pixel.r;
    }
}


[CustomEditor(typeof(TerrainStampDesignerTool))]
public class TerrainStampDesignerToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainStampDesignerTool tool = (TerrainStampDesignerTool)target;
        if (GUILayout.Button("Adjust Terrain"))
        {
            tool.AdjustTerrain();
        }
    }
}