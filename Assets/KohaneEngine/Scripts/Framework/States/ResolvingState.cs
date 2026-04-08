using KohaneEngine.Scripts.Story;
using KohaneEngine.Scripts.Structure;

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
            var ret = _storyManager.ResolveNext();
            if (ret == ResultType.Choice)
            {
                return;
            }

            StateManager.TransitionTo<AnimatingState>();
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
        }
    }
}