using System;
using System.Collections.Generic;
using DG.Tweening;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class ImageResolver : Resolver
    {
        private static readonly int SecondTex = Shader.PropertyToID("_SecondTex");
        private static readonly int Lerp = Shader.PropertyToID("_Lerp");
        private readonly IResourceManager _resourceManager;
        private readonly KohaneBinder _binder;
        private readonly KohaneAnimator _animator;

        private readonly Dictionary<string, RawImage> _images = new();
        private readonly Dictionary<string, Tween> _imagePersistentEffects = new();

        public ImageResolver(IResourceManager resourceManager, KohaneBinder binder, KohaneAnimator animator)
        {
            _resourceManager = resourceManager;
            _binder = binder;
            _animator = animator;
            Functions.Add("__imgDefine", ImageDefine);
            Functions.Add("__imgDelete", ImageDelete);
            Functions.Add("imgSwitch", ImageSwitch);
            Functions.Add("imgMove", ImageMove);
            Functions.Add("imgScale", ImageScale);
            Functions.Add("imgAlpha", ImageAlpha);
            Functions.Add("imgEffect", ImageEffect);
        }

        [StoryFunctionAttr("imgScale")]
        private ResolveResult ImageScale(Block block)
        {
            var id = block.GetArg<string>(0);
            var ax = block.GetArg<float>(1);
            var ay = block.GetArg<float>(2);
            var tween = block.GetArg<int>(3);
            var dur = block.GetArg<float>(4);
            _animator.AppendAnimation(GetImage(id).rectTransform
                .DOScale(new Vector3(ax, ay, 1), dur).SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("imgAlpha")]
        private ResolveResult ImageAlpha(Block block)
        {
            var id = block.GetArg<string>(0);
            var alpha = block.GetArg<float>(1);
            var tween = block.GetArg<int>(2);
            var dur = block.GetArg<float>(3);
            _animator.AppendAnimation(GetImage(id)
                .DOFade(alpha, dur).SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("imgMove")]
        private ResolveResult ImageMove(Block block)
        {
            var id = block.GetArg<string>(0);
            var ax = block.GetArg<float>(1);
            var ay = block.GetArg<float>(2);
            var tween = block.GetArg<int>(3);
            var dur = block.GetArg<float>(4);
            _animator.AppendAnimation(GetImage(id).rectTransform
                .DOAnchorPos(UIUtils.ScriptPositionToCanvasPosition(new Vector2(ax, ay)), dur)
                .SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("imgSwitch")]
        private ResolveResult ImageSwitch(Block block)
        {
            var id = block.GetArg<string>(0);
            SetImage(GetImage(id), block.GetArg<string>(1), block.GetArg<float>(2));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("__imgDelete")]
        private ResolveResult ImageDelete(Block block)
        {
            var id = block.GetArg<string>(0);
            if (!_images.ContainsKey(id))
            {
                return ResolveResult.FailResult(
                    "[ImageResolver] Deleting undefined image, are you using builtin function???");
            }
            
            // Or use a pool if you are a nerd
            _images.Remove(id);
            if (_imagePersistentEffects.ContainsKey(id))
            {
                _imagePersistentEffects[id].Kill();
                _imagePersistentEffects.Remove(id);
            }
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("__imgDefine")]
        private ResolveResult ImageDefine(Block block)
        {
            var id = block.GetArg<string>(0);
            var layer = block.GetArg<int>(1);
            if (_images.ContainsKey(id))
            {
                return ResolveResult.FailResult(
                    "[ImageResolver] Image already defined, are you using builtin function???");
            }

            var uiCanvas = _binder.CreateImage();
            uiCanvas.name = $"Image - {id}";
            uiCanvas.sortingOrder = layer;
            
            var newImg = uiCanvas.GetComponentInChildren<RawImage>();
            newImg.material = new Material(Shader.Find("Unlit/AlphaBlend"));
            _images.Add(id, newImg);
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("imgEffect")]
        private ResolveResult ImageEffect(Block block)
        {
            var id = block.GetArg<string>(0);
            var effectName = block.GetArg<string>(1);
            var dur = block.GetArg<float>(2);

            if (effectName == "clear")
            {
                if (_imagePersistentEffects.ContainsKey(id))
                {
                    _imagePersistentEffects[id].Kill();
                    _imagePersistentEffects.Remove(id);
                }

                return ResolveResult.SuccessResult();
            }

            var persistent = dur <= 0;

            if (persistent)
            {
                // Set a constant period
                dur = 1;
            }

            Tween effectTween;

            switch (effectName)
            {
                case "shakeX":
                    effectTween = GetImage(id).rectTransform
                        .DOShakeAnchorPos(dur, new Vector2(20, 0), 20, 90, false, false, ShakeRandomnessMode.Harmonic);
                    break;
                case "shakeY":
                    effectTween = GetImage(id).rectTransform
                        .DOShakeAnchorPos(dur, new Vector2(0, 20), 20, 90, false, false, ShakeRandomnessMode.Harmonic);
                    break;
                case "shake":
                    effectTween = GetImage(id).rectTransform
                        .DOShakeAnchorPos(dur, new Vector2(20, 20), 20, 90, false, false, ShakeRandomnessMode.Harmonic);
                    break;
                default:
                    throw new InvalidOperationException($"[CharacterResolver] Invalid effect name: {effectName}");
            }

            if (persistent)
            {
                effectTween.SetLoops(-1);
                _imagePersistentEffects[id] = effectTween;
            }
            else
            {
                _animator.AppendAnimation(effectTween);
            }

            return ResolveResult.SuccessResult();
        }

        private RawImage GetImage(string id)
        {
            if (!_images.TryGetValue(id, out var image))
            {
                throw new InvalidOperationException(
                    "[ImageResolver] Invalid image id, are you using builtin function???");
            }

            return image;
        }

        // TODO: implement async loading
        private void SetImage(RawImage img, string path, float duration)
        {
            var nextImage = _resourceManager.LoadResource<Texture>(string.Format(Constants.ImagePath,
                path));
            if (!img.texture)
            {
                img.texture = nextImage;
                img.SetNativeSize();
                //img.material.SetFloat(Progress, 0);
                return;
            }

            //var transitionImage = img.transform.GetChild(0).GetComponent<RawImage>();

     

            // _animator.AppendTweenAnimation(img.DOFade(0,
            //     Constants.ImageCrossFadeDuration), true);
            // _animator.JoinTweenAnimation(transitionImage.DOFade(1,
            //     Constants.ImageCrossFadeDuration));
            _animator.AppendAnimation(img.material.DOFloat(1, Lerp, duration).OnStart(() =>
            {
                img.material.SetTexture(SecondTex, nextImage);
                // transitionImage.texture = nextImage;
                // transitionImage.SetNativeSize();
            }).OnComplete(() =>
            {
                img.material.SetTexture(SecondTex, null);
                img.material.SetFloat(Lerp, 0);
                img.texture = nextImage;
                img.SetNativeSize();
                // transitionImage.color = new Color(1, 1, 1, 0);
            }));
        }
    }
}