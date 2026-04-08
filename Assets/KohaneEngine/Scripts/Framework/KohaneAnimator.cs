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

        private DefaultMode _defaultMode = DefaultMode.Parallel;

        private float _insertAt = float.NaN;
        private float _waitCounter = 0f;

        public void InsertNextAnimationAt(float time)
        {
            if (_defaultMode == DefaultMode.Sequence)
            {
                throw new Exception("Sequence mode does not support @at");
            }

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

            if (!float.IsNaN(_insertAt))
            {
                _tweenSequence.Insert(_insertAt, tween);
                _insertAt = float.NaN;
                return;
            }

            if (_defaultMode == DefaultMode.Parallel && !forceAppend)
            {
                _tweenSequence.Insert(_waitCounter, tween);
            }
            else
            {
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

            if (!float.IsNaN(_insertAt))
            {
                _tweenSequence.InsertCallback(_insertAt, wrappedCallback);
                _insertAt = float.NaN;
                return;
            }

            if (_defaultMode == DefaultMode.Parallel && !forceAppend)
            {
                _tweenSequence.InsertCallback(_waitCounter, wrappedCallback);
            }
            else
            {
                _tweenSequence.AppendCallback(wrappedCallback);
            }
        }

        public void AppendTweenInterval(float duration)
        {
            CheckTweenSequence();
            _waitCounter += duration;
            if (_defaultMode == DefaultMode.Sequence)
            {
                _tweenSequence.AppendInterval(duration);
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
            }
        }

        public void InterruptAnimation()
        {
            _pendingCallbacks.ForEach(p => p?.Invoke());
            _tweenSequence.Complete();
        }

        public void SetSequenceMode(bool val)
        {
            _defaultMode = val ? DefaultMode.Sequence : DefaultMode.Parallel;
        }

        public void RequireAfter()
        {
            _waitCounter = _tweenSequence.Duration();
        }

        private void CheckTweenSequence()
        {
            if (_tweenSequence.IsActive() || _tweenSequence.IsPlaying())
            {
                return;
            }

            _pendingCallbacks.Clear();
            _waitCounter = 0;
            _tweenSequence = DOTween.Sequence();
        }
    }

    enum DefaultMode
    {
        Sequence,
        Parallel
    }
}