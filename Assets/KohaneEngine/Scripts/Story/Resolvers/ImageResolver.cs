using System;
using System.Collections.Generic;
using DG.Tweening;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class ImageResolver : Resolver
    {
        private readonly IResourceManager _resourceManager;
        private readonly KohaneBinder _binder;
        private readonly KohaneAnimator _animator;

        private readonly Dictionary<string, RawImage> _images = new();

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
        }

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

        private ResolveResult ImageSwitch(Block block)
        {
            var id = block.GetArg<string>(0);
            SetImage(GetImage(id), block.GetArg<string>(1));
            return ResolveResult.SuccessResult();
        }

        //TODO: Destroy used image or use an object pool
        private ResolveResult ImageDelete(Block block)
        {
            var id = block.GetArg<string>(0);
            if (!_images.ContainsKey(id))
            {
                return ResolveResult.FailResult("[ImageResolver] Deleting undefined image, are you using builtin function???");
            }
            _images.Remove(id);
            return ResolveResult.SuccessResult();
        }

        private ResolveResult ImageDefine(Block block)
        {
            var id = block.GetArg<string>(0);
            var layer = block.GetArg<int>(1);
            if (_images.ContainsKey(id))
            {
                return ResolveResult.FailResult("[ImageResolver] Image already defined, are you using builtin function???");
            }

            var newImg = _binder.CreateImage();
            newImg
            _images.Add(id, );
            return ResolveResult.SuccessResult();
        }

        private RawImage GetImage(string id)
        {
            if (!_images.TryGetValue(id, out var image))
            {
                throw new InvalidOperationException("[ImageResolver] Invalid image id, are you using builtin function???");
            }
            return image;
        }

        //TODO: implement async loading
        private void SetImage(RawImage img, string path)
        {
            var nextImage = _resourceManager.LoadResource<Texture>(string.Format(Constants.ImagePath,
                    path));
            if (!img.texture)
            {
                img.texture = nextImage;
                img.SetNativeSize();
                img.transform.GetChild(0).GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
                //img.material.SetFloat(Progress, 0);
                return;
            }

            var transitionImage = img.transform.GetChild(0).GetComponent<RawImage>();

            _animator.AppendCallback(() =>
            {
                transitionImage.texture = nextImage;
                transitionImage.SetNativeSize();
            }, true);
            
            // _animator.AppendTweenAnimation(img.DOFade(0,
            //     Constants.ImageCrossFadeDuration), true);
            // _animator.JoinTweenAnimation(transitionImage.DOFade(1,
            //     Constants.ImageCrossFadeDuration));
            var tempAlpha = 0f;
            var targetAlpha = img.color.a;
            _animator.AppendAnimation(DOTween.To(() => tempAlpha, (x) =>
            {
                tempAlpha = x;
                img.color = new Color(1, 1, 1, targetAlpha - x);
                transitionImage.color = new Color(1, 1, 1, x);
            }, targetAlpha, Constants.CrossFadeDuration), true);
            
            _animator.AppendCallback(() =>
            {
                img.texture = nextImage;
                img.SetNativeSize();
                img.color = Color.white;
                transitionImage.color = new Color(1, 1, 1, 0);
            }, true);
        }
    }
}