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
        }
        
        public override ResolveResult Resolve(Block block)
        {
            switch (block.type)
            {
                case "blackScreen":
                    var alpha = block.GetArg<float>(0);
                    var tween = block.GetArg<int>(1);
                    var dur = block.GetArg<float>(2);
                    _animator.AppendAnimation(_blackScreen.DOFade(alpha, dur).SetEase((Ease) tween));
                    break;
                case "showDialogBox":
                    alpha = block.GetArg<float>(0);
                    tween = block.GetArg<int>(1);
                    dur = block.GetArg<float>(2);
                    _animator.AppendAnimation(_dialogCanvas.DOFade(alpha, dur).SetEase((Ease) tween));
                    break;
            }
            return ResolveResult.SuccessResult();
        }
    }
}