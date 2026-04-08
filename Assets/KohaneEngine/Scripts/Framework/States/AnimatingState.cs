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
            _animator.StartAnimation(() => { StateManager.TransitionTo(new ReadyState(StateManager)); });
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
            _animator.InterruptAnimation();
        }
    }
}