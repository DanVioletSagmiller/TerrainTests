using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Force Graph Connections Layer")]
public class ForceGraphConnectionsModLayer : TerrainModLayer
{
   

    public void OnValidate()
    {
        Tool.OnValidate();
    }

    public override void Rebuild() 
    { 
        
    }

    public override void Apply()
    {
        
    }
}
