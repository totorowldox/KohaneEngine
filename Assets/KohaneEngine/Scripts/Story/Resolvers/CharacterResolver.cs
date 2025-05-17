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

        private readonly Dictionary<string, RawImage> _characterImages = new();

        public CharacterResolver(IResourceManager resourceManager, KohaneBinder binder, KohaneAnimator animator)
        {
            _resourceManager = resourceManager;
            _binder = binder;
            _animator = animator;
            Functions.Add("__charDefine", CharDefine);
            Functions.Add("__charDelete", CharDelete);
            Functions.Add("charSwitch", CharSwitch);
            Functions.Add("charMove", CharMove);
            Functions.Add("charScale", CharScale);
            Functions.Add("charAlpha", CharAlpha);
        }

        [StoryFunctionAttr("charScale")]
        private ResolveResult CharScale(Block block)
        {
            var id = block.GetArg<string>(0);
            var ax = block.GetArg<float>(1);
            var ay = block.GetArg<float>(2);
            var tween = block.GetArg<int>(3);
            var dur = block.GetArg<float>(4);
            _animator.AppendAnimation(GetCharacterImage(id).rectTransform
                .DOScale(new Vector3(ax, ay, 1), dur).SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("charAlpha")]
        private ResolveResult CharAlpha(Block block)
        {
            var id = block.GetArg<string>(0);
            var alpha = block.GetArg<float>(1);
            var tween = block.GetArg<int>(2);
            var dur = block.GetArg<float>(3);
            _animator.AppendAnimation(GetCharacterImage(id)
                .DOFade(alpha, dur).SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("charMove")]
        private ResolveResult CharMove(Block block)
        {
            var id = block.GetArg<string>(0);
            var ax = block.GetArg<float>(1);
            var ay = block.GetArg<float>(2);
            var tween = block.GetArg<int>(3);
            var dur = block.GetArg<float>(4);
            _animator.AppendAnimation(GetCharacterImage(id).rectTransform
                .DOAnchorPos(UIUtils.ScriptPositionToCanvasPosition(new Vector2(ax, ay)), dur)
                .SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("charSwitch")]
        private ResolveResult CharSwitch(Block block)
        {
            var id = block.GetArg<string>(0);
            SetCharacterImage(GetCharacterImage(id), block.GetArg<string>(1));
            return ResolveResult.SuccessResult();
        }

        // TODO: destroy used character or use an object pool
        [StoryFunctionAttr("__charDelete")]
        private ResolveResult CharDelete(Block block)
        {
            var id = block.GetArg<string>(0);
            if (!_characterImages.ContainsKey(id))
            {
                return ResolveResult.FailResult("[CharacterResolver] Deleting undefined character, are you using builtin function???");
            }
            _characterImages.Remove(id);
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("__charDefine")]
        private ResolveResult CharDefine(Block block)
        {
            var id = block.GetArg<string>(0);
            if (_characterImages.ContainsKey(id))
            {
                return ResolveResult.FailResult("[CharacterResolver] Character already defined, are you using builtin function???");
            }

            var newChar = _binder.CreateCharacterImage();
            newChar.name = $"Char - {id}";
            _characterImages.Add(id, newChar);
            return ResolveResult.SuccessResult();
        }

        private RawImage GetCharacterImage(string id)
        {
            if (!_characterImages.TryGetValue(id, out var characterImage))
            {
                throw new InvalidOperationException("[CharacterResolver] Invalid character id, are you using builtin function???");
            }
            return characterImage;
        }

        // TODO: implement async loading
        private void SetCharacterImage(RawImage characterImage, string path)
        {
            var nextImage = _resourceManager.LoadResource<Texture>(string.Format(Constants.CharacterPath,
                    path));
            if (!characterImage.texture)
            {
                characterImage.texture = nextImage;
                characterImage.SetNativeSize();
                characterImage.transform.GetChild(0).GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
                //characterImage.material.SetFloat(Progress, 0);
                return;
            }

            var transitionImage = characterImage.transform.GetChild(0).GetComponent<RawImage>();

            _animator.AppendCallback(() =>
            {
                transitionImage.texture = nextImage;
                transitionImage.SetNativeSize();
            }, true);
            
            // _animator.AppendTweenAnimation(characterImage.DOFade(0,
            //     Constants.CharacterCrossFadeDuration), true);
            // _animator.JoinTweenAnimation(transitionImage.DOFade(1,
            //     Constants.CharacterCrossFadeDuration));
            var tempAlpha = 0f;
            var targetAlpha = characterImage.color.a;
            _animator.AppendAnimation(DOTween.To(() => tempAlpha, (x) =>
            {
                tempAlpha = x;
                characterImage.color = new Color(1, 1, 1, targetAlpha - x);
                transitionImage.color = new Color(1, 1, 1, x);
            }, targetAlpha, Constants.CrossFadeDuration), true);
            
            _animator.AppendCallback(() =>
            {
                characterImage.texture = nextImage;
                characterImage.SetNativeSize();
                characterImage.color = Color.white;
                transitionImage.color = new Color(1, 1, 1, 0);
            }, true);
        }
    }
}