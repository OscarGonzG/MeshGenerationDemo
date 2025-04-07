using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct CubicBezierNode2D
{
    public Vector2 startHandle;
    public Vector2 point;
    public Vector2 endHandle;
}

public class CubicBezier2D
{
    private List<CubicBezierNode2D> cubicBezierNodes = new List<CubicBezierNode2D>();

    public Vector2 EvaluatePoint(float t)
    {
        if (t > cubicBezierNodes.Count)
        {
            throw new System.ArgumentOutOfRangeException("t must be in range [0, number of nodes]");
        }

        CubicBezierNode2D node1 = cubicBezierNodes[(int) t];
        CubicBezierNode2D node2 = cubicBezierNodes[(int) t + 1];

        ref Vector2 p1 = ref node1.point;
        ref Vector2 h1 = ref node1.endHandle;
        ref Vector2 p2 = ref node2.point;
        ref Vector2 h2 = ref node2.startHandle;

        Vector2 q1 = Vector2.Lerp(p1, h1, t);
        Vector2 q2 = Vector2.Lerp(h1, h2, t);
        Vector2 q3 = Vector2.Lerp(h2, p2, t);

        Vector2 r1 = Vector2.Lerp(q1, q2, t);
        Vector2 r2 = Vector2.Lerp(q2, q3, t);

        return Vector2.Lerp(r1, r2, t);
    }

}
