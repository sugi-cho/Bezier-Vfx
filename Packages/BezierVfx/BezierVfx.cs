using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace sugi.vfx.bezier
{
    public class BezierVfx : MonoBehaviour
    {
        [SerializeField] VisualEffect vfx;
        [SerializeField] bool closed;
        [SerializeField] int maxBezierSegments = 512;

        bool edit;
        GraphicsBuffer m_buffer;
        List<BezierSegment> m_bezierSegments;

        public List<BezierSegment> BezierSegments
        {
            get => m_bezierSegments;
            set {
                m_bezierSegments = value;
                UpdateVfx();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            m_bezierSegments.ForEach(s
               => Handles.DrawBezier(s.p0, s.p3, s.p1, s.p2, Color.blue, null, 2f));
        }
#endif

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
    }
}