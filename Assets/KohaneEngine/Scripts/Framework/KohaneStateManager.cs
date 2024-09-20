using System;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    public class KohaneStateManager
    {
        private KohaneState CurrentState { get; set; } = KohaneState.None;
        private KohaneFlag CurrentFlags { get; set; } = KohaneFlag.None;

        private int _asyncLayers;

        public bool SwitchState(KohaneState nextState)
        {
            //TODO
            switch (nextState)
            {
                case KohaneState.Resolving:
                    if (!CanResolve())
                    {
                        Debug.LogWarning("<color=#e198b4>[StateManager]</color> Unable to set to resolving state!");
                        return false;
                    }
                    break;
            }

            CurrentState = nextState;
            Debug.Log("<color=#e198b4>[StateManager]</color> Current state: " + CurrentState);
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

        private bool CanResolve() => (IsInState(KohaneState.Ready) || IsInState(KohaneState.None)) && 
                                     !HasFlag(KohaneFlag.InSystem);

        private bool CanSkip() => !HasFlag(KohaneFlag.CannotSkip) &&
                                  !HasFlag(KohaneFlag.InSystem);
    }

    [Flags]
    public enum KohaneState
    {
        None = 0,
        Ready = 1 << 0,
        Resolving = 1 << 1,
        ResolveEnd = 1 << 2,
    }
    
    [Flags]
    public enum KohaneFlag
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