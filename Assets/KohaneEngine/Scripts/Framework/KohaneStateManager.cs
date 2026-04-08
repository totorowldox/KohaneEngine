using System;
using KohaneEngine.Scripts.Framework.States;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    public class KohaneStateManager : MonoBehaviour
    {
        private KohaneState CurrentState { get; set; }
        private KohaneFlag CurrentFlags { get; set; } = KohaneFlag.None;

        public PlaybackMode CurrentPlayback { get; private set; } = PlaybackMode.Normal;

        private void Awake()
        {
            TransitionTo<ReadyState>();
        }

        private void Update()
        {
            CurrentState?.OnUpdate();
        }

        public void TransitionTo(KohaneState newState)
        {
            Debug.Log($"Transitioning from {CurrentState} to {newState}");
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState.OnEnter();
        }

        public void TransitionTo<T>() where T : KohaneState
        {
            TransitionTo((T) Activator.CreateInstance(typeof(T), new object[] {this}));
        }

        public bool AddFlag(KohaneFlag flag)
        {
            if (flag == KohaneFlag.CannotSkip)
            {
                CurrentPlayback = PlaybackMode.Normal;
            }

            CurrentFlags |= flag;
            return true;
        }

        public bool RemoveFlag(KohaneFlag flag)
        {
            CurrentFlags &= ~flag;
            return true;
        }

        public void SetPlayback(PlaybackMode mode)
        {
            if (CurrentPlayback == mode) return;
            CurrentPlayback = mode;
            // Broadcast to UI elements / reactive
        }

        public bool HasFlag(KohaneFlag flag) => CurrentFlags.HasFlag(flag);
        public bool IsSkipping => CurrentPlayback == PlaybackMode.Skip;
        public bool IsAuto => CurrentPlayback == PlaybackMode.Auto;
    }

    [Flags]
    public enum KohaneFlag
    {
        None = 1,
        CannotSkip = 1 << 1,
    }

    public enum PlaybackMode
    {
        Normal,
        Auto,
        Skip,
        Loading
    }
}