using KohaneEngine.Scripts.Story;

namespace KohaneEngine.Scripts.Framework.States
{
    public class ResolvingState : KohaneState
    {
        private readonly KohaneStoryManager _storyManager;

        public ResolvingState(KohaneStateManager stateManager) : base(stateManager)
        {
            _storyManager = KohaneEngine.Resolver.Resolve<KohaneStoryManager>();
        }

        public override void OnEnter()
        {
            _storyManager.ResolveNext();
            StateManager.TransitionTo(new AnimatingState(StateManager));
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
        }
    }
}