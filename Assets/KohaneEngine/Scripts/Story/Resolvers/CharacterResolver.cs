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
    public class CharacterResolver : Resolver
    {
        private static readonly int SecondTex = Shader.PropertyToID("_SecondTex");
        private static readonly int Lerp = Shader.PropertyToID("_Lerp");
        private readonly IResourceManager _resourceManager;
        private readonly KohaneBinder _binder;
        private readonly KohaneAnimator _animator;

        private readonly Dictionary<string, RawImage> _characterImages = new();
        private readonly Dictionary<string, Tween> _characterPersistentEffects = new();

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
            Functions.Add("charEffect", CharEffect);
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
            SetCharacterImage(GetCharacterImage(id), block.GetArg<string>(1), block.GetArg<float>(2));
            return ResolveResult.SuccessResult();
        }

        // TODO: destroy used character or use an object pool
        [StoryFunctionAttr("__charDelete")]
        private ResolveResult CharDelete(Block block)
        {
            var id = block.GetArg<string>(0);
            if (!_characterImages.ContainsKey(id))
            {
                return ResolveResult.FailResult(
                    "[CharacterResolver] Deleting undefined character, are you using builtin function???");
            }
            
            // Or use a pool if you are a nerd
            _characterImages.Remove(id);
            if (_characterPersistentEffects.ContainsKey(id))
            {
                _characterPersistentEffects[id].Kill();
                _characterPersistentEffects.Remove(id);
            }

            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("__charDefine")]
        private ResolveResult CharDefine(Block block)
        {
            var id = block.GetArg<string>(0);
            if (_characterImages.ContainsKey(id))
            {
                return ResolveResult.FailResult(
                    "[CharacterResolver] Character already defined, are you using builtin function???");
            }

            var newChar = _binder.CreateCharacterImage();
            newChar.name = $"Char - {id}";
            newChar.material = new Material(Shader.Find("Unlit/AlphaBlend"));
            _characterImages.Add(id, newChar);
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("charEffect")]
        private ResolveResult CharEffect(Block block)
        {
            var id = block.GetArg<string>(0);
            var effectName = block.GetArg<string>(1);
            var dur = block.GetArg<float>(2);

            if (effectName == "clear")
            {
                if (_characterPersistentEffects.ContainsKey(id))
                {
                    _characterPersistentEffects[id].Kill();
                    _characterPersistentEffects.Remove(id);
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
                    effectTween = GetCharacterImage(id).rectTransform
                        .DOShakeAnchorPos(dur, new Vector2(20, 0), 20, 90, false, false, ShakeRandomnessMode.Harmonic);
                    break;
                case "shakeY":
                    effectTween = GetCharacterImage(id).rectTransform
                        .DOShakeAnchorPos(dur, new Vector2(0, 20), 20, 90, false, false, ShakeRandomnessMode.Harmonic);
                    break;
                case "shake":
                    effectTween = GetCharacterImage(id).rectTransform
                        .DOShakeAnchorPos(dur, new Vector2(20, 20), 20, 90, false, false, ShakeRandomnessMode.Harmonic);
                    break;
                default:
                    throw new InvalidOperationException($"[CharacterResolver] Invalid effect name: {effectName}");
            }

            if (persistent)
            {
                effectTween.SetLoops(-1);
                _characterPersistentEffects.Add(id, effectTween);
            }
            else
            {
                _animator.AppendAnimation(effectTween);
            }

            return ResolveResult.SuccessResult();
        }


        private RawImage GetCharacterImage(string id)
        {
            if (!_characterImages.TryGetValue(id, out var characterImage))
            {
                throw new InvalidOperationException(
                    "[CharacterResolver] Invalid character id, are you using builtin function???");
            }

            return characterImage;
        }

        // TODO: implement async loading
        private void SetCharacterImage(RawImage characterImage, string path, float duration)
        {
            var nextImage = _resourceManager.LoadResource<Texture>(string.Format(Constants.CharacterPath,
                path));
            if (!characterImage.texture)
            {
                characterImage.texture = nextImage;
                characterImage.SetNativeSize();
                //characterImage.material.SetFloat(Progress, 0);
                return;
            }

            //var transitionImage = characterImage.transform.GetChild(0).GetComponent<RawImage>();


            // _animator.AppendAnimation(characterImage.DOFade(0,
            //     Constants.CrossFadeDuration), true);
            // _animator.JoinAnimation(transitionImage.DOFade(1,
            //     Constants.CrossFadeDuration));
            _animator.AppendAnimation(characterImage.material.DOFloat(1, Lerp, duration).OnStart(() =>
            {
                characterImage.material.SetTexture(SecondTex, nextImage);
                // transitionImage.texture = nextImage;
                // transitionImage.SetNativeSize();
            }).OnComplete(() =>
            {
                characterImage.material.SetTexture(SecondTex, null);
                characterImage.material.SetFloat(Lerp, 0);
                characterImage.texture = nextImage;
                characterImage.SetNativeSize();
                // transitionImage.color = new Color(1, 1, 1, 0);
            }));
        }
    }
}