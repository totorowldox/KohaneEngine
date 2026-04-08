namespace KohaneEngine.Scripts.Framework.States
{
    public class AnimatingState : KohaneState
    {
        private readonly KohaneAnimator _animator;

        public AnimatingState(KohaneStateManager stateManager) : base(stateManager)
        {
            _animator = KohaneEngine.Resolver.Resolve<KohaneAnimator>();
        }

        public override void OnEnter()
        {
            _animator.StartAnimation(() => { StateManager.TransitionTo<ReadyState>(); });
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnExit()
        {
        }

        protected override void RequestNextStep()
        {
            if (StateManager.HasFlag(KohaneFlag.CannotSkip))
            {
                return;
            }

            _animator.InterruptAnimation();
        }
    }
}