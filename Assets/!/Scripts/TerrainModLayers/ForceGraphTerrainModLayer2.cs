//using UnityEngine;

//[CreateAssetMenu(menuName = "Terrain/Layers/Force Graph2")]
//public class ForceGraphTerrainModLayer2 : TerrainModLayer
//{
//    public ForceGraphVisualizerManager ForceGraphiVisualizer;

//    public float Radius = 50f;

//    private float[,] Mesh;
//    private float[,] MeshStrength;

//    public void OnValidate()
//    {
//        Tool.OnValidate();
//    }

//    public override void Rebuild()
//    {
//        if (this.ForceGraphiVisualizer == null)
//        {
//            this.ForceGraphiVisualizer = GameObject.FindObjectOfType<ForceGraphVisualizerManager>();   
//        }

//        Mesh = new float[Tool.HeightRes, Tool.HeightRes];
//        MeshStrength = new float[Tool.HeightRes, Tool.HeightRes];
//        var settings = this.ForceGraphiVisualizer.Settings;

//        var anchor = this.ForceGraphiVisualizer.transform.position;

//        var height = Tool.GetTerrainPercentForWorldHeight(anchor.y);
//        var res = Tool.HeightRes;
//        for (int x = 0; x < res; x++)
//        {
//            for (int y = 0; y < res; y++)
//            {
//                foreach (var node in this.ForceGraphiVisualizer.ForceGraph.Nodes)
//                {
//                    var pos = node.Position * settings.Scaling + anchor;

//                    var distance = ZeroDistance(
//                        pos,
//                        Tool.WorldPositionFromHeightMapIndex(x, y));

//                    if (distance < Radius)
//                    {
//                        Mesh[x, y] = height;
//                        MeshStrength[x, y] = 1f;
//                        continue;
//                    }
//                }
//            }
//        }
//    }

//    private float ZeroDistance(Vector3 v1, Vector3 v2)
//    {
//        v1.y = 0;
//        v2.y = 0;
//        return Vector3.Distance(v1, v2);
//    }

//    public override void Apply()
//    {
//        if (Mesh == null) Rebuild();

//        for(int x = 0; x < Tool.HeightRes; x++)
//        {
//            for(int y = 0; y < Tool.HeightRes; y++)
//            {
//                var overlayStrength = MeshStrength[x, y];
//                var originalStrength = 1f - overlayStrength;
//                Tool.Mesh[x, y] = 
//                    (originalStrength * Tool.Mesh[x,y]) 
//                    + (overlayStrength * Mesh[x, y]);
//            }
//        }
//    }
//}
