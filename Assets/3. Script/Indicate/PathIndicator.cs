using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathIndicator : MonoBehaviour
{
    public Transform currentPosition;
    public LineRenderer lineRenderer;
    public NavMeshAgent agent;

    void Start()
    {
        lineRenderer.alignment = LineAlignment.TransformZ;
    }

    void Update()
    {
        agent.SetDestination(currentPosition.position);
        DrawPath();
    }

    void DrawPath()
    {
        NavMeshPath path = agent.path;
        List<Vector3> pathPoints = new List<Vector3>();

        if (path.corners.Length < 2)
            return;

        for (int i = 0; i < path.corners.Length; i++)
        {
            pathPoints.Add(path.corners[i]);
        }

        lineRenderer.positionCount = pathPoints.Count;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, pathPoints[i]);
        }
    }
}
