using UnityEngine;

public class TerrainFlattener : MonoBehaviour
{
    public TerrainTool TerrainTools;
    public float Radius = 50f;

    public void RefreshContent()
    {
        var height = TerrainTools.GetTerrainPercentForWorldHeight(transform.position.y);
        var res = TerrainTools.Mesh.GetLength(0);
        for(int x = 0; x < res; x++)
        {
            for(int y = 0; y < res; y++)
            {
                var distance = ZeroDistance(
                    transform.position,
                    TerrainTools.WorldPositionFromHeightMapIndex(x, y));

                if (distance < Radius)
                {
                    TerrainTools.Mesh[x, y] = height;
                }
            }
        }
    }

    private float ZeroDistance(Vector3 v1, Vector3 v2)
    {
        v1.y = 0;
        v2.y = 0;
        return Vector3.Distance(v1, v2);
    }
}
