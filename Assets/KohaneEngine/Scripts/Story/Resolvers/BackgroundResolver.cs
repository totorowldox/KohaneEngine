﻿using System.Threading.Tasks;
using DG.Tweening;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class BackgroundResolver : Resolver
    {
        private readonly IResourceManager _resourceManager;
        private readonly KohaneAnimator _animator;

        private readonly Image _backgroundImage;

        public BackgroundResolver(IResourceManager resourceManager, KohaneBinder binder, KohaneAnimator animator)
        {
            _resourceManager = resourceManager;
            _backgroundImage = binder.backgroundImage;
            _animator = animator;
            Functions.Add("__bgSwitch", BgSwitch);
            Functions.Add("__bgRemove", BgRemove);
            Functions.Add("bgMove", BgMove);
            Functions.Add("bgScale", BgScale);
            Functions.Add("bgAlpha", BgAlpha);
        }

        [StoryFunctionAttr("bgScale")]
        private ResolveResult BgScale(Block block)
        {
            var ax = block.GetArg<float>(0);
            var ay = block.GetArg<float>(1);
            var tween = block.GetArg<int>(2);
            var dur = block.GetArg<float>(3);
            _animator.AppendAnimation(_backgroundImage.rectTransform
                .DOScale(new Vector3(ax, ay, 1), dur)
                .SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("bgMove")]
        private ResolveResult BgMove(Block block)
        {
            var ax = block.GetArg<float>(0);
            var ay = block.GetArg<float>(1);
            var tween = block.GetArg<int>(2);
            var dur = block.GetArg<float>(3);
            _animator.AppendAnimation(_backgroundImage.rectTransform
                .DOAnchorPos(UIUtils.ScriptPositionToCanvasPosition(new Vector2(ax, ay)), dur)
                .SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("bgAlpha")]
        private ResolveResult BgAlpha(Block block)
        {
            var alpha = block.GetArg<float>(0);
            var tween = block.GetArg<int>(1);
            var dur = block.GetArg<float>(2);
            _animator.AppendAnimation(_backgroundImage
                .DOFade(alpha, dur).SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("__bgRemove")]
        private ResolveResult BgRemove(Block block)
        {
            _backgroundImage.sprite = null;
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("__bgSwitch")]
        private ResolveResult BgSwitch(Block block)
        {
            _animator.AppendCallback(() => SetBackgroundImage(block.GetArg<string>(0)));
            return ResolveResult.SuccessResult();
        }

        private async Task SetBackgroundImage(string path)
        {
            var nextImage =
                await _resourceManager.LoadResourceAsync<Sprite>(string.Format(Constants.BackgroundPath,
                    path));
            
            _backgroundImage.sprite = nextImage;
        }
    }
}