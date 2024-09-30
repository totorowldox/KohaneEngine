using System;
using System.Collections.Generic;
using DG.Tweening;

namespace KohaneEngine.Scripts.Framework
{
    /// <summary>
    /// Manager of all animations
    /// </summary>
    public class KohaneAnimator
    {
        private Sequence _tweenSequence = DOTween.Sequence();
        private readonly List<TweenCallback> _pendingCallbacks = new();

        private KohaneStateManager _stateManager = null;
        private KohaneStateManager StateManager => _stateManager ??= KohaneEngine.Resolver.Resolve<KohaneStateManager>();
        private bool _isAsync = false;

        private float _insertAt = float.NaN;

        public void InsertNextAnimationAt(float time)
        {
            _insertAt = time;
        }

        public void AppendAnimation(Tween tween, bool forceAppend = false)
        {
            CheckTweenSequence();
            if (StateManager.HasFlag(KohaneFlag.AsyncResolving))
            {
                if (_isAsync)
                {
                    _tweenSequence.Join(tween);
                }
                else
                {
                    _tweenSequence.Append(tween);
                    _isAsync = true;
                }
            }
            else
            {
                if (!float.IsNaN(_insertAt))
                {
                    _tweenSequence.Insert(_insertAt, tween);
                    _insertAt = float.NaN;
                    return;
                }
                _isAsync = forceAppend && _isAsync;
                _tweenSequence.Append(tween);
            }
        }
        
        public void JoinAnimation(Tween tween)
        {
            CheckTweenSequence();
            _tweenSequence.Join(tween);
        }

        public void AppendCallback(TweenCallback callback, bool forceAppend = false)
        {
            CheckTweenSequence();
            TweenCallback wrappedCallback = () =>
            {
                callback?.Invoke();
                _pendingCallbacks.Remove(callback);
            };
            _pendingCallbacks.Add(callback);
            if (StateManager.HasFlag(KohaneFlag.AsyncResolving) && !forceAppend)
            {
                if (_isAsync)
                {
                    _tweenSequence.JoinCallback(wrappedCallback);
                }
                else
                {
                    _tweenSequence.AppendCallback(wrappedCallback);
                    _isAsync = true;
                }
            }
            else
            {
                if (!float.IsNaN(_insertAt))
                {
                    _tweenSequence.InsertCallback(_insertAt, wrappedCallback);
                    _insertAt = float.NaN;
                    return;
                }
                _isAsync = forceAppend && _isAsync;
                _tweenSequence.AppendCallback(wrappedCallback);
            }
        }

        public void AppendTweenInterval(float duration)
        {
            CheckTweenSequence();
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
            var addedDelay = StateManager.HasFlag(KohaneFlag.Auto) ? Constants.AutoSpeed : 0f;
            addedDelay = StateManager.HasFlag(KohaneFlag.Skip) ? Constants.SkipSpeed : addedDelay;

            if (!shouldComplete && StateManager.AddFlag(KohaneFlag.Animating))
            {
                _tweenSequence.AppendInterval(addedDelay);
                _tweenSequence.OnComplete(delegate
                {
                    _pendingCallbacks.ForEach(p => p?.Invoke());
                    StateManager.RemoveFlag(KohaneFlag.Animating);
                    StateManager.SwitchState(KohaneState.Ready);
                });
                _tweenSequence.Play();
            }
            else
            {
                _pendingCallbacks.ForEach(p => p?.Invoke());
                _tweenSequence.Complete();
                _tweenSequence = DOTween.Sequence();
                _tweenSequence.AppendInterval(addedDelay);
                _tweenSequence.OnComplete(delegate
                {
                    StateManager.SwitchState(KohaneState.Ready);
                });
                _tweenSequence.Play();
            }
        }

        public void InterruptAnimation()
        {
            _pendingCallbacks.ForEach(p => p?.Invoke());
            _tweenSequence.Complete();
        }

        private void CheckTweenSequence()
        {
            if (_tweenSequence.IsActive() || _tweenSequence.IsPlaying())
            {
                return;
            }
            _pendingCallbacks.Clear();
            _tweenSequence = DOTween.Sequence();
        }
    }
}