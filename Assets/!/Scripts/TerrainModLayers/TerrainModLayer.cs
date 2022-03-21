public abstract class TerrainModLayer : UnityEngine.ScriptableObject
{
    [UnityEngine.HideInInspector]
    public TerrainTool Tool;
    public abstract void Apply();
    public abstract void Rebuild();
}
