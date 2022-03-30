﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Layers/Force Graph Connections Layer")]
public class ForceGraphConnectionsModLayer : TerrainModLayer
{
    public float PathWidth = 5f;
    public AnimationCurve PathWallShape;
    private float[,] Mesh = null;
    private float[,] MeshStrength = null;


    public void OnValidate()
    {
        Tool.OnValidate();
    }

   

    public override void Rebuild()
    {
        var forceGraph = Tool.GetLayer<ForceGraphTerrainModLayer>();
        if (forceGraph == null)
        {
            Debug.LogError("TerrainTools is currently setup with ForceGraphConnectionsModLayer, but that requires ForceGraphTerrainModLayer is also available.");
        }

        var content = new List<Vector3[]>();


        foreach (var node in forceGraph.ForceGraph.Nodes)
        {
            foreach (var node2 in node.ConnectedNodes)
            {
                if (node.Id > node2.Id) continue;
                content.Add(new Vector3[] 
                { 
                    node.Position * forceGraph.Settings.Scaling, 
                    node2.Position * forceGraph.Settings.Scaling
                });
            }
        }

        Mesh = new float[Tool.HeightRes, Tool.HeightRes];
        MeshStrength = new float[Tool.HeightRes, Tool.HeightRes];
        var anchor = ForceGraphVisualizerManager.Instance.transform.position;
        var height = Tool.GetTerrainPercentForWorldHeight(anchor.y);

        for (int x = 0; x < Tool.HeightRes; x++)
        {
            for (int y = 0; y < Tool.HeightRes; y++)
            {
                var p = Tool.WorldPositionFromHeightMapIndex(x, y);
                foreach (var c in content)
                {
                    var closestPoint = Helper.ClosestPointOnLineSegment(
                        p, 
                        c[0] + anchor, 
                        c[1] + anchor);
                    var distance = Vector3.Distance(p, closestPoint);
                        //UnityEditor.HandleUtility.DistancePointToLineSegment(p, c[0], c[1]);
                    if (distance > PathWidth) continue;

                    var percent = distance / PathWidth;
                    var appliedPercent = this.PathWallShape.Evaluate(percent);
                    Mesh[x, y] = height;
                    MeshStrength[x, y] += appliedPercent;
                }

                if (MeshStrength[x, y] > 1f) MeshStrength[x, y] = 1f;
            }
        }
    }

    public override void Apply()
    {
        if (Mesh == null || MeshStrength == null)
        {
            Rebuild();
        }

        for (int x = 0; x < Tool.HeightRes; x++)
        {
            for (int y = 0; y < Tool.HeightRes; y++)
            {
                var overlayStrength = MeshStrength[x, y];
                var originalStrength = 1f - overlayStrength;
                Tool.Mesh[x, y] =
                    (originalStrength * Tool.Mesh[x, y])
                    + (overlayStrength * Mesh[x, y]);
            }
        }
    }
}
