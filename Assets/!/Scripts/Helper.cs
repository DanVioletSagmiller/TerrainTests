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
        var v = position - lineA;
        var dot = Vector3.Dot(v, normal);
        dot = Mathf.Clamp(dot, 0f, length);
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