using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace KohaneEngine.Scripts.Graphic.TypewriterAnimation
{
    public class FadeDownTypewriterAnimation : TypewriterAnimation
    {
        private const float AnimationDuration = .5f;
        private const float FloatAmplitude = 60;
        private const Ease EaseFunction = Ease.OutQuart;

        public FadeDownTypewriterAnimation(KohaneBinder binder)
        {
            TextContainer = binder.text;
            SpeakerContainer = binder.speaker;
        }
        
        public override void InitializeAnimation(string speaker, string text)
        {
            TextContainer.ClearMesh(true);
            TextContainer.SetText(text);
            TextContainer.ForceMeshUpdate();
            SpeakerContainer.SetText(speaker);
        }

        public override void UpdateAnimation(float phase)
        {
            TextContainer.ForceMeshUpdate();
            var textInfo = TextContainer.textInfo;
            var matIndexSet = new HashSet<int>();
            for (var t = 0; t < textInfo.characterCount; t++)
            {
                var c = textInfo.characterInfo[t];
                if (!c.isVisible)
                {
                    continue;
                }
                var matIndex = c.materialReferenceIndex;
                matIndexSet.Add(matIndex);
                var index = c.vertexIndex;

                var startTime = t * Constants.TypeAnimationSpeed;
                var orgPercent = Math.Clamp((phase - startTime) / AnimationDuration, 0, 1);
                var percent = DOVirtual.EasedValue(0, 1, orgPercent, EaseFunction);
                var inversePercent = 1 - percent;

                // Update vertices
                var vertices = textInfo.meshInfo[matIndex].vertices;
                vertices[index + 0] += new Vector3(0, inversePercent * FloatAmplitude, 0);
                vertices[index + 1] += new Vector3(0, inversePercent * FloatAmplitude, 0);
                vertices[index + 2] += new Vector3(0, inversePercent * FloatAmplitude, 0);
                vertices[index + 3] += new Vector3(0, inversePercent * FloatAmplitude, 0);

                // Update colors
                var nextColor = Color32.Lerp(new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 255), percent);
                var colors = textInfo.meshInfo[matIndex].colors32;
                colors[index + 0] = nextColor;
                colors[index + 1] = nextColor;
                colors[index + 2] = nextColor;
                colors[index + 3] = nextColor;
            }
            foreach (var i in matIndexSet)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                TextContainer.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            TextContainer.UpdateVertexData();
        }

        public override float GetDuration(string text)
        {
            var ret = (TextContainer.GetTextInfo(text).characterCount - 1) *
                Constants.TypeAnimationSpeed + AnimationDuration;
            TextContainer.SetText("");
            return ret;
        }
    }
}