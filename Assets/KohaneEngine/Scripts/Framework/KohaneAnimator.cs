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

        private KohaneStateManager StateManager =>
            _stateManager ??= KohaneEngine.Resolver.Resolve<KohaneStateManager>();

        private bool _isAsync = false;
        private int _asyncLayers = 0;

        private float _insertAt = float.NaN;

        public void InsertNextAnimationAt(float time)
        {
            _insertAt = time;
        }

        public void AppendAnimation(Tween tween, bool forceAppend = false)
        {
            CheckTweenSequence();

            if (StateManager.IsSkipping)
            {
                tween.Complete();
                return;
            }

            if (_asyncLayers > 0)
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
            if (StateManager.IsSkipping)
            {
                tween.Complete();
                return;
            }

            _tweenSequence.Join(tween);
        }

        public void AppendCallback(TweenCallback callback, bool forceAppend = false)
        {
            CheckTweenSequence();

            if (StateManager.IsSkipping)
            {
                callback?.Invoke();
                return;
            }

            TweenCallback wrappedCallback = () =>
            {
                callback?.Invoke();
                _pendingCallbacks.Remove(callback);
            };
            _pendingCallbacks.Add(callback);
            if (_asyncLayers > 0 && !forceAppend)
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
            if (_asyncLayers == 0)
            {
                _tweenSequence.AppendInterval(duration);
            }
            else
            {
                throw new InvalidOperationException("Should not have wait command during async operation");
            }
        }

        public void StartAnimation(Action endCallback)
        {
            var shouldComplete = StateManager.IsSkipping;

            if (!shouldComplete)
            {
                _tweenSequence.OnComplete(delegate
                {
                    _pendingCallbacks.ForEach(p => p?.Invoke());
                    endCallback.Invoke();
                });
                _tweenSequence.Play();
            }
            else
            {
                endCallback.Invoke();
                return;
                _pendingCallbacks.ForEach(p => p?.Invoke());
                _tweenSequence.Complete();
                _tweenSequence = DOTween.Sequence();
                _tweenSequence.OnComplete(delegate { endCallback.Invoke(); });
                _tweenSequence.Play();
            }
        }

        public void InterruptAnimation()
        {
            _pendingCallbacks.ForEach(p => p?.Invoke());
            _tweenSequence.Complete();
        }

        public void AddAsyncLayer()
        {
            _asyncLayers++;
        }

        public void ReduceAsyncLayer()
        {
            _asyncLayers = Math.Max(0, _asyncLayers - 1);
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