using System.Linq;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Framework.States;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class ProgressResolver : Resolver
    {
        private readonly KohaneStateManager _stateManager;
        private readonly KohaneStoryManager _storyManager;

        public ProgressResolver(KohaneStateManager stateManager, KohaneStoryManager storyManager)
        {
            _stateManager = stateManager;
            _storyManager = storyManager;
            Functions.Add("selection", Selection);
            Functions.Add("jumpToScene", JumpToScene);
        }

        [StoryFunctionAttr("selection")]
        private ResolveResult Selection(Block block)
        {
            var selections = block.GetArg<string>(0).Split('|')
                .Select(s => s.Split(':'))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

            _stateManager.TransitionTo(new ChoiceState(_stateManager, selections));
            return ResolveResult.ChoiceResult();
        }

        [StoryFunctionAttr("jumpToScene")]
        private ResolveResult JumpToScene(Block arg)
        {
            _storyManager.JumpToScene(arg.GetArg<string>(0));
            return ResolveResult.SuccessResult();
        }
    }
}