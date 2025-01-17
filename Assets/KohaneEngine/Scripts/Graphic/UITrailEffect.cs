using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KohaneEngine.Scripts.Graphic
{
    public class UITrailEffect : BaseMeshEffect
    {
        [SerializeField] private int trailLength = 10;
        [SerializeField] private float fadeDuration = 1.0f;
        [SerializeField] private float staggering = 0.05f;

        private readonly List<UIVertex[]> _trailVertices = new ();
        private readonly List<float> _trailTimes = new ();
        private readonly List<Vector3> _trailPositions = new ();
        private float _lastTime = 0f;

        private bool ShouldUpdate { get => transform.hasChanged; set => transform.hasChanged = value; }

        protected override void Awake()
        {
            base.Awake();
            ShouldUpdate = false;
        }

        private void Update()
        {
            // Update the time for each trail segment
            for (var i = 0; i < _trailTimes.Count; i++)
            {
                _trailTimes[i] += Time.deltaTime;
            }

            // Remove old trail segments that have exceeded the fade duration
            while (_trailTimes.Count > 0 && _trailTimes[^1] > fadeDuration)
            {
                _trailTimes.RemoveAt(_trailTimes.Count - 1);
                _trailVertices.RemoveAt(_trailVertices.Count - 1);
                _trailPositions.RemoveAt(_trailPositions.Count - 1);
            }

            // Mark the UI element for update
            if (ShouldUpdate || _trailVertices.Count > 0)
            {
                graphic.SetVerticesDirty(); // or SetLayoutDirty();
            }
    
            _lastTime += Time.deltaTime;
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0) return;

            // Get the original vertices
            var verts = new List<UIVertex>();
            vh.GetUIVertexStream(verts);
            
            // Remove old trail positions to maintain the trail length
            if (_trailVertices.Count > trailLength)
            {
                _trailVertices.RemoveAt(0);
                _trailTimes.RemoveAt(0);
            }
            else if (ShouldUpdate && _lastTime >= staggering)
            {
                ShouldUpdate = false;
                _lastTime = 0f;
                _trailVertices.Insert(0, verts.ToArray());
                _trailTimes.Insert(0, 0.0f);
                _trailPositions.Insert(0, transform.localPosition);
            }

            // Modify the vertices to create the trail effect
            for (var i = _trailVertices.Count - 1; i >= 0; i--)
            {
                var t = _trailTimes[i] / fadeDuration;
                var color = Color.Lerp(graphic.color - new Color(0, 0, 0, 0.7f), new Color(1, 1, 1, 0), t);
                var relativePos = DivideVector(_trailPositions[i] - transform.localPosition, transform.localScale);

                for (var j = 0; j < _trailVertices[i].Length; j++)
                {
                    var vert = _trailVertices[i][j];
                    vert.color = color;
                    vert.position += relativePos;
                    verts.Add(vert);
                }

                continue;

                Vector3 DivideVector(Vector3 a, Vector3 b) => new(a.x / b.x, a.y / b.y, a.z / b.z);
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }
    }
}
