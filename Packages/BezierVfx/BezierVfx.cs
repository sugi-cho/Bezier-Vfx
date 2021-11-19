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
        [SerializeField] string bufferPropName = "BezierBuffer";
        [SerializeField] string numPropName = "nSegments";
        [SerializeField] int maxBezierSegments = 512;
        [SerializeField] SegmentType segmentType = SegmentType._2D;

        GraphicsBuffer m_buffer;
        List<BezierSegment2D> m_bezierSegments2d;
        List<BezierSegment3D> m_bezierSegments3d;

        int numDimensions => (int)segmentType;

        public List<BezierSegment2D> BezierSegments2D
        {
            get => m_bezierSegments2d;
            set
            {
                if (segmentType == SegmentType._2D)
                {
                    m_bezierSegments2d = value;
                    UpdateVfx();
                }
                else
                    Debug.LogWarning($"segmentType({segmentType}) is not match {SegmentType._2D}");
            }
        }
        public List<BezierSegment3D> BezierSegments3D
        {
            get => m_bezierSegments3d;
            set
            {
                if (segmentType == SegmentType._3D)
                {
                    m_bezierSegments3d = value;
                    UpdateVfx();
                }
                else
                    Debug.LogWarning($"segmentType({segmentType}) is not match {SegmentType._3D}");
            }
        }
        /// <summary>
        /// Set Data Array Directory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void SetData<T>(T[] data)
        {
            if (m_buffer == null)
                m_buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 4 * maxBezierSegments, sizeof(float) * numDimensions);
            m_buffer.SetData(data);
            vfx.SetGraphicsBuffer(bufferPropName, m_buffer);
            vfx.SetInt(numPropName, data.Length);
        }

        private void OnEnable()
        {
            m_buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 4 * maxBezierSegments, sizeof(float) * numDimensions);
            vfx.SetGraphicsBuffer(bufferPropName, m_buffer);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            m_bezierSegments3d.ForEach(s
               => Handles.DrawBezier(s.p0, s.p3, s.p1, s.p2, Color.blue, null, 2f));
        }
        private void OnValidate()
        {
            if (m_buffer != null)
                m_buffer.Release();
            m_buffer = null;
        }
#endif

        void UpdateVfx()
        {
            if (m_buffer == null)
                m_buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 4 * maxBezierSegments, sizeof(float) * numDimensions);
            switch (segmentType)
            {
                case SegmentType._2D:
                    m_buffer.SetData(m_bezierSegments2d);
                    break;
                case SegmentType._3D:
                    m_buffer.SetData(m_bezierSegments3d);
                    break;
            }
            vfx.SetGraphicsBuffer(bufferPropName, m_buffer);
            vfx.SetInt(numPropName, segmentType == SegmentType._2D ? m_bezierSegments2d.Count : m_bezierSegments3d.Count);
        }

        [System.Serializable]
        public struct BezierSegment2D
        {
            public Vector2 p0;
            public Vector2 p1;
            public Vector2 p2;
            public Vector2 p3;
        }
        [System.Serializable]
        public struct BezierSegment3D
        {
            public Vector3 p0;
            public Vector3 p1;
            public Vector3 p2;
            public Vector3 p3;
        }

        public enum SegmentType
        {
            _2D = 2,
            _3D = 3,
        }
    }
}