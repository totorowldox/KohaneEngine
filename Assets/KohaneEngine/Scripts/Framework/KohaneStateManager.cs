using System;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    public class KohaneStateManager
    {
        private KohaneState CurrentState { get; set; } = KohaneState.Ready;
        private KohaneFlag CurrentFlags { get; set; } = KohaneFlag.None;

        private int _asyncLayers = 0;

        public bool SwitchState(KohaneState nextState)
        {
            //TODO
            switch (nextState)
            {
                case KohaneState.Resolving:
                    if (!CanResolve())
                        return false;
                    break;
            }

            CurrentState = nextState;
            Debug.Log("[StateManager] Current state: " + CurrentState);
            return true;
        }

        public bool AddFlag(KohaneFlag flag)
        {
            switch (flag)
            {
                case KohaneFlag.AsyncResolving:
                    _asyncLayers++;
                    break;
            }
            CurrentFlags |= flag;
            return true;
        }
        
        public bool RemoveFlag(KohaneFlag flag)
        {
            switch (flag)
            {
                case KohaneFlag.AsyncResolving:
                    _asyncLayers--;
                    if (_asyncLayers == 0)
                    {
                        CurrentFlags &= ~flag;
                    }
                    return true;
            }
            CurrentFlags &= ~flag;
            return true;
        }

        public bool IsInState(KohaneState state) => CurrentState.HasFlag(state);
        
        public bool HasFlag(KohaneFlag flag) => CurrentFlags.HasFlag(flag);

        private bool CanResolve() => CurrentState.HasFlag(KohaneState.Ready) && !CurrentFlags.HasFlag(KohaneFlag.InSystem);

        private bool CanSkip() => !CurrentFlags.HasFlag(KohaneFlag.CannotSkip) &&
                                  !CurrentFlags.HasFlag(KohaneFlag.InSystem);
    }

    [Flags]
    public enum KohaneState : int
    {
        Ready = 0,
        Resolving = 1 << 0,
        WaitingForClick = 1 << 1,
    }
    
    [Flags]
    public enum KohaneFlag : int
    {
        None = 0,
        Skip = 1 << 1,
        Auto = 1 << 2,
        InSystem = 1 << 3,
        AsyncResolving = 1 << 4,
        CannotSkip = 1 << 5,
        Animating = 1 << 6,
    }
}