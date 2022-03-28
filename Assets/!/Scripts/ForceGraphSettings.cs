using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Force Graph Settings")]
public class ForceGraphSettings : ScriptableObject
{
    public float Scaling = 1f;
    public float MoveForce = .01f;
    public int Iterations = 100;
    public AnimationCurve BlendingOverDistance;
}
