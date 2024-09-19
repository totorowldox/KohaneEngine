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
    public class CharacterResolver : Resolver
    {
        private readonly IResourceManager _resourceManager;
        private readonly KohaneBinder _binder;
        private readonly KohaneAnimator _animator;

        private readonly Dictionary<string, Image> _characterImages = new();

        public CharacterResolver(IResourceManager resourceManager, KohaneBinder binder, KohaneAnimator animator)
        {
            _resourceManager = resourceManager;
            _binder = binder;
            _animator = animator;
        }
        
        public override ResolveResult Resolve(Block block)
        {
            var id = block.GetArg<string>(0);
            switch (block.type)
            {
                case "__charDefine":
                    if (_characterImages.ContainsKey(id))
                    {
                        return ResolveResult.FailResult();
                    }
                    _characterImages.Add(id, _binder.CreateCharacterImage());
                    break;
                case "__charDelete":
                    if (!_characterImages.Remove(id))
                    {
                        return ResolveResult.FailResult("deleting undefined character");
                    }
                    break;
                case "charSwitch":
                    SetCharacterImage(GetCharacterImage(id), block.GetArg<string>(1));
                    break;
                case "charMove":
                    var ax = block.GetArg<float>(1);
                    var ay = block.GetArg<float>(2);
                    var tween = block.GetArg<int>(3);
                    var dur = block.GetArg<float>(4);
                    _animator.AppendTweenAnimation(GetCharacterImage(id).rectTransform
                        .DOAnchorPos(UIUtils.ScriptPositionToCanvasPosition(new Vector2(ax, ay)), dur)
                        .SetEase((Ease) tween));
                    break;
                case "charAlpha":
                    var alpha = block.GetArg<float>(1);
                    tween = block.GetArg<int>(2);
                    dur = block.GetArg<float>(3);
                    _animator.AppendTweenAnimation(GetCharacterImage(id)
                        .DOFade(alpha, dur).SetEase((Ease) tween));
                    break;
                case "charScale":
                    ax = block.GetArg<float>(1);
                    ay = block.GetArg<float>(2);
                    tween = block.GetArg<int>(3);
                    dur = block.GetArg<float>(4);
                    _animator.AppendTweenAnimation(GetCharacterImage(id).rectTransform
                        .DOScale(new Vector2(ax, ay), dur).SetEase((Ease) tween));
                    break;
            }
            return ResolveResult.SuccessResult();
        }

        private Image GetCharacterImage(string id)
        {
            if (!_characterImages.TryGetValue(id, out var characterImage))
            {
                throw new InvalidOperationException("Invalid character id, are you using builtin function???");
            }
            return characterImage;
        }

        public async void SetCharacterImage(Image characterImage, string path)
        {
            var nextImage =
                await _resourceManager.LoadResourceAsync<Sprite>(string.Format(Constants.CharacterPath,
                    path));
            
            characterImage.sprite = nextImage;
            characterImage.SetNativeSize();
        }
    }
}