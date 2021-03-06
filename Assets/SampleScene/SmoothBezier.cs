using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using sugi.vfx.bezier;
using BezierSegment3D = sugi.vfx.bezier.BezierVfx.BezierSegment3D;

[ExecuteInEditMode]
public class SmoothBezier : MonoBehaviour
{
    [SerializeField] SmoothBezierSegment[] smoothBeziers;
    [SerializeField] bool closed;
    [SerializeField] BezierVfx bezierVfx;

    bool edit;
    List<BezierSegment3D> m_bezierSegments = new List<BezierSegment3D>();

    private void OnValidate()
    {
        m_bezierSegments.Clear();
        for (var i = 0; i < smoothBeziers.Length + (closed ? 0 : -1); i++)
        {
            var s0 = smoothBeziers[i];
            var s1 = smoothBeziers[(i + 1) % smoothBeziers.Length];
            var segment = new BezierSegment3D { p0 = s0.p0, p1 = s0.p1, p2 = 2 * s1.p0 - s1.p1, p3 = s1.p0 };
            m_bezierSegments.Add(segment);
        }
        edit = true;
    }

    private void Update()
    {
        if (edit)
            bezierVfx.BezierSegments3D = m_bezierSegments;
        edit = false;
    }

    [System.Serializable]
    public struct SmoothBezierSegment
    {
        public Vector3 p0;
        public Vector3 p1;
    }
}
