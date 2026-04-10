using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace KohaneEngine.Scripts.UI
{
    public class TransitionLayer : MonoBehaviour
    {
        private static readonly int Progress1 = Shader.PropertyToID("_Progress");
        private static readonly int PatternTex = Shader.PropertyToID("_PatternTex");
        private static readonly int InvertY = Shader.PropertyToID("_InvertY");
        private RawImage _rawImage;
        private Camera _mainCamera;
        [SerializeField] private Shader transitionShader;
        [SerializeField] private List<Texture2D> masks;

        [CanBeNull] private RenderTexture _tempRT;

        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _rawImage.material = new Material(transitionShader);
            //_rawImage.material.SetFloat(InvertY, 1.0f);
            _mainCamera = Camera.main;
        }

        public void PrepareTransition()
        {
            // Screen capture
            _tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            //ScreenCapture.CaptureScreenshotIntoRenderTexture(_tempRT);
            _mainCamera.targetTexture = _tempRT;
            _mainCamera.Render();
            _mainCamera.targetTexture = null;

            // Set render texture
            _rawImage.texture = _tempRT;
            _rawImage.material.SetFloat(Progress1, 1);
            _rawImage.color = new Color(0, 0, 0, 1);
        }

        public Tween StartTransition(int type, int tween, float time)
        {
            return _rawImage.material.DOFloat(0, Progress1, time).SetEase((Ease) tween).OnComplete(() =>
            {
                if (_tempRT == null)
                {
                    return;
                }

                _rawImage.texture = null;
                RenderTexture.ReleaseTemporary(_tempRT);
                _tempRT = null;
            }).OnStart(() => { _rawImage.material.SetTexture(PatternTex, masks[type]); });
        }

        public Tween FadeOut(int tween, float time)
        {
            _rawImage.color = new Color(0, 0, 0, 0);
            _rawImage.material.SetFloat(Progress1, 0);
            return _rawImage.material.DOFloat(1, "_Alpha", time).SetEase((Ease) tween);
        }
    }
}