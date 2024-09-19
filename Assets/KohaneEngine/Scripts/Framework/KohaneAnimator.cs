using System;
using DG.Tweening;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    /// <summary>
    /// Manager of all animations
    /// </summary>
    public class KohaneAnimator
    {
        private Sequence _tweenSequence = DOTween.Sequence();

        private KohaneStateManager _stateManager = null;
        private KohaneStateManager StateManager => _stateManager ??= KohaneEngine.Resolver.Resolve<KohaneStateManager>();

        public void AppendTweenAnimation(Tween tween)
        {
            if (!_tweenSequence.IsActive() && !_tweenSequence.IsPlaying())
            {
                _tweenSequence = DOTween.Sequence();
            }
            
            if (StateManager.HasFlag(KohaneFlag.AsyncResolving))
            {
                _tweenSequence.Join(tween);
            }
            else
            {
                _tweenSequence.Append(tween);
            }
        }

        public void AppendTweenInterval(float duration)
        {
            if (!StateManager.HasFlag(KohaneFlag.AsyncResolving))
            {
                _tweenSequence.AppendInterval(duration);
            }
            else
            {
                throw new InvalidOperationException("Should not have wait command during async operation");
            }
        }

        public void StartAnimation()
        {
            var shouldComplete = StateManager.HasFlag(KohaneFlag.Skip);

            if (!shouldComplete && StateManager.AddFlag(KohaneFlag.Animating))
            {
                _tweenSequence.OnComplete(delegate
                {
                    StateManager.RemoveFlag(KohaneFlag.Animating);
                    StateManager.SwitchState(KohaneState.Ready);
                });
                _tweenSequence.Play();
            }
            else
            {
                _tweenSequence.Complete();
                StateManager.SwitchState(KohaneState.Ready);
            }
        }

        public void InterruptAnimation()
        {
            _tweenSequence.Complete();
        }
    }
}