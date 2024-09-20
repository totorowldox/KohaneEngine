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
        }
        
        public override ResolveResult Resolve(Block block)
        {
            switch (block.type)
            {
                case "__bgSwitch":
                    _animator.AppendCallback(() => SetBackgroundImage(block.GetArg<string>(0)));
                    break;
                case "__bgRemove":
                    _backgroundImage.sprite = null;
                    break;
                case "bgAlpha":
                    var alpha = block.GetArg<float>(0);
                    var tween = block.GetArg<int>(1);
                    var dur = block.GetArg<float>(2);
                    _animator.AppendTweenAnimation(_backgroundImage
                        .DOFade(alpha, dur).SetEase((Ease) tween));
                    break;
                case "bgMove":
                    var ax = block.GetArg<float>(0);
                    var ay = block.GetArg<float>(1);
                    tween = block.GetArg<int>(2);
                    dur = block.GetArg<float>(3);
                    _animator.AppendTweenAnimation(_backgroundImage.rectTransform
                        .DOAnchorPos(UIUtils.ScriptPositionToCanvasPosition(new Vector2(ax, ay)), dur)
                        .SetEase((Ease) tween));
                    break;
            }
            return ResolveResult.SuccessResult();
        }
        
        public async void SetBackgroundImage(string path)
        {
            var nextImage =
                await _resourceManager.LoadResourceAsync<Sprite>(string.Format(Constants.BackgroundPath,
                    path));
            
            _backgroundImage.sprite = nextImage;
        }
    }
}