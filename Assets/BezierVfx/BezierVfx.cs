using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class BezierVfx : MonoBehaviour
{
    [SerializeField] VisualEffect vfx;
    [SerializeField] SmoothBezierSegment[] smoothBeziers;
    [SerializeField] bool closed;
    [SerializeField] int maxBezierSegments = 512;

    bool edit;
    GraphicsBuffer m_buffer;
    List<BezierSegment> m_bezierSegments = new List<BezierSegment>();

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        m_bezierSegments.ForEach(s
           => Handles.DrawBezier(s.p0, s.p3, s.p1, s.p2, Color.blue, null, 2f));
    }
#endif
    private void OnValidate()
    {
        m_bezierSegments.Clear();
        for (var i = 0; i < smoothBeziers.Length + (closed ? 0 : -1); i++)
        {
            var s0 = smoothBeziers[i];
            var s1 = smoothBeziers[(i + 1) % smoothBeziers.Length];
            var segment = new BezierSegment { p0 = s0.p0, p1 = s0.p1, p2 = 2 * s1.p0 - s1.p1, p3 = s1.p0 };
            m_bezierSegments.Add(segment);
        }
        edit = true;
    }

    private void Update()
    {
        if (edit)
            UpdateVfx();
        edit = false;
    }

    void UpdateVfx()
    {
        if (m_buffer == null)
            m_buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 4 * maxBezierSegments, sizeof(float) * 3);
        m_buffer.SetData(m_bezierSegments);
        vfx.SetGraphicsBuffer("BezierBuffer", m_buffer);
        vfx.SetInt("nSegments", m_bezierSegments.Count);
    }

    [System.Serializable]
    public struct BezierSegment
    {
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
    }
    [System.Serializable]
    public struct SmoothBezierSegment
    {
        public Vector3 p0;
        public Vector3 p1;
    }
}
