namespace KohaneEngine.Scripts.Framework.States
{
    public abstract class KohaneState
    {
        protected readonly KohaneStateManager StateManager;
        protected readonly KohaneInputManager InputManager;

        protected KohaneState(KohaneStateManager stateManager)
        {
            StateManager = stateManager;
            InputManager = KohaneEngine.Resolver.Resolve<KohaneInputManager>();
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
            HandleInput();
        }

        public virtual void OnExit()
        {
        }

        protected virtual void HandleInput()
        {
            if (!StateManager.HasFlag(KohaneFlag.CannotSkip) && HandleSkipInput()) return;

            HandleAutoToggleInput();

            HandleNextStepInput();
        }

        protected virtual bool HandleSkipInput()
        {
            if (InputManager.GetInputDown("skip"))
            {
                StateManager.SetPlayback(PlaybackMode.Skip);
                RequestNextStep();
                return true;
            }

            if (InputManager.GetInputUp("skip"))
            {
                StateManager.SetPlayback(PlaybackMode.Normal);
            }

            return false;
        }

        protected virtual void HandleAutoToggleInput()
        {
            if (!InputManager.GetPointerUp("auto"))
            {
                return;
            }

            if (StateManager.IsAuto)
            {
                StateManager.SetPlayback(PlaybackMode.Normal);
            }
            else
            {
                StateManager.SetPlayback(PlaybackMode.Auto);
                RequestNextStep();
            }
        }

        protected virtual void HandleNextStepInput()
        {
            var wantsNext = InputManager.GetInputUp("next_step") ||
                            InputManager.GetPointerUp("next_step");

            if (!wantsNext)
            {
                return;
            }

            if (StateManager.IsAuto)
                StateManager.SetPlayback(PlaybackMode.Normal);

            RequestNextStep();
        }

        protected virtual void RequestNextStep()
        {
            StateManager.TransitionTo<ResolvingState>();
        }
    }
}