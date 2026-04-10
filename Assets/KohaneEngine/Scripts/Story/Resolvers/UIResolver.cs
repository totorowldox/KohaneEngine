using DG.Tweening;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.UI;
using KohaneEngine.Scripts.Utils;
using UnityEngine;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class UIResolver : Resolver
    {
        private readonly IResourceManager _resourceManager;
        private readonly KohaneAnimator _animator;

        private readonly TransitionLayer _transitionLayer;
        private readonly ScreenEffectManager _screenEffectManager;
        private readonly CanvasGroup _dialogCanvas;

        public UIResolver(IResourceManager resourceManager, KohaneBinder binder, KohaneAnimator animator)
        {
            _resourceManager = resourceManager;
            _transitionLayer = binder.transitionLayer;
            _dialogCanvas = binder.dialogCanvasGroup;
            _screenEffectManager = binder.screenEffectManager;
            _animator = animator;
            Functions.Add("showDialogBox", ShowDialogBox);
            Functions.Add("blackScreen", BlackScreen);
            Functions.Add("prepareTransition", PrepareTransition);
            Functions.Add("startTransition", StartTransition);
            Functions.Add("effect", SetScreenEffect);
        }

        [StoryFunctionAttr("showDialogBox")]
        private ResolveResult ShowDialogBox(Block block)
        {
            var alpha = block.GetArg<float>(0);
            var tween = block.GetArg<int>(1);
            var dur = block.GetArg<float>(2);
            _animator.AppendAnimation(_dialogCanvas.DOFade(alpha, dur).SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("blackScreen")]
        private ResolveResult BlackScreen(Block block)
        {
            var alpha = block.GetArg<float>(0);
            var tween = block.GetArg<int>(1);
            var dur = block.GetArg<float>(2);
            _animator.AppendAnimation(_transitionLayer.FadeOut(tween, dur));
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("prepareTransition")]
        private ResolveResult PrepareTransition(Block block)
        {
            _animator.AppendCallback(() => { _transitionLayer.PrepareTransition(); }, true);
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("startTransition")]
        private ResolveResult StartTransition(Block block)
        {
            var type = block.GetArg<int>(0);
            var tween = block.GetArg<int>(1);
            var dur = block.GetArg<float>(2);
            _animator.AppendAnimation(_transitionLayer.StartTransition(type, tween, dur));
            return ResolveResult.SuccessResult();
        }


        [StoryFunctionAttr("effect")]
        private ResolveResult SetScreenEffect(Block block)
        {
            var type = block.GetArg<string>(0);
            var duration = block.GetArg<float>(1);
            _screenEffectManager.PlayEffect(type, duration, _animator);

            return ResolveResult.SuccessResult();
        }
    }
}