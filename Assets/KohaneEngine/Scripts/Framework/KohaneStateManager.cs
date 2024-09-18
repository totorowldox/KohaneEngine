using System;

namespace KohaneEngine.Scripts.Framework
{
    public class KohaneStateManager
    {
        private KohaneState CurrentState { get; set; } = KohaneState.Ready;

        public bool SwitchState(KohaneState nextState)
        {
            //TODO
            switch (nextState)
            {
                case KohaneState.Resolving:
                    if (!CanResolve())
                        return false;
                    break;
                case KohaneState.Skipping:
                    if (!CanSkip())
                        return false;
                    break;
            }

            CurrentState = nextState;
            return true;
        }

        public bool IsInState(KohaneState state) => CurrentState.HasFlag(state);

        private bool CanResolve() => CurrentState.HasFlag(KohaneState.Ready) && !CurrentState.HasFlag(KohaneState.InSystem);

        private bool CanSkip() => !CurrentState.HasFlag(KohaneState.CannotSkip) &&
                                  !CurrentState.HasFlag(KohaneState.InSystem);
    }

    [Flags]
    public enum KohaneState : int
    {
        Ready = 0,
        Resolving = 1 << 0,
        WaitingForClick = 1 << 1,
        Animating = 1 << 2,
        Skipping = 1 << 3,
        CannotSkip = 1 << 4,
        InSystem = 1 << 5
    }
}