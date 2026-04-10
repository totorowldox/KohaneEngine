using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KohaneEngine.Scripts.Framework;
using UnityEngine;

namespace KohaneEngine.Scripts.UI
{
    public class ScreenEffectManager : MonoBehaviour
    {
        private Dictionary<string, GameObject> _effectDict;

        private void Awake()
        {
            _effectDict = new Dictionary<string, GameObject>
            {
                ["speedLine"] = transform.Find("Speed Line").gameObject,
                ["speedLineH"] = transform.Find("Horizontal Speed Line").gameObject
            };
        }

        public void PlayEffect(string type, float duration, KohaneAnimator animator)
        {
            if (type == "clear")
            {
                animator.AppendCallback(() =>
                {
                    foreach (var effect in _effectDict.Values)
                    {
                        effect.SetActive(false);
                    }
                });

                return;
            }

            if (!_effectDict.TryGetValue(type, out var effectGameObject))
            {
                throw new Exception($"Effect {type} not found");
            }

            if (duration <= 0)
            {
                animator.AppendCallback(() => { effectGameObject.SetActive(true); });
                return;
            }

            animator.AppendAnimation(DOTween.To(() => 0, value => _ = value, 0, duration)
                .OnStart(() => { effectGameObject.SetActive(true); })
                .OnComplete(() => { effectGameObject.SetActive(false); }));
        }
    }
}