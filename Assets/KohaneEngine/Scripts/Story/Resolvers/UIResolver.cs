using DG.Tweening;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.ResourceManager;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class UIResolver : Resolver
    {
        private readonly IResourceManager _resourceManager;
        private readonly KohaneAnimator _animator;

        private readonly Image _blackScreen;
        private readonly CanvasGroup _dialogCanvas;

        public UIResolver(IResourceManager resourceManager, KohaneBinder binder, KohaneAnimator animator)
        {
            _resourceManager = resourceManager;
            _blackScreen = binder.blackScreenImage;
            _dialogCanvas = binder.dialogCanvasGroup;
            _animator = animator;
            Functions.Add("showDialogBox", ShowDialogBox);
            Functions.Add("blackScreen", BlackScreen);
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
            _animator.AppendAnimation(_blackScreen.DOFade(alpha, dur).SetEase((Ease) tween));
            return ResolveResult.SuccessResult();
        }
    }
}