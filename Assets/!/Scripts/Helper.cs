using UnityEngine;

public class Helper
{
    public static Vector3 ClosestPointOnLineSegment(
        Vector3 position, 
        Vector3 lineA, 
        Vector3 lineB)
    {
        var diff = lineB - lineA;
        var length = diff.magnitude;
        var normal = diff.normalized;
        var diffA = position - lineA;
        var dot = Mathf.Clamp(Vector3.Dot(diffA, normal), 0f, length);
        return lineA + normal * dot;
    }

    public static Vector3 PointInLineSegment(
        float percentFromA,
        Vector3 lineA, 
        Vector3 lineB)
    {
        var diff = lineB - lineA;
        return lineA + diff * percentFromA;
    }
}