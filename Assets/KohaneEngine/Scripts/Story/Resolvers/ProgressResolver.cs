using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;

namespace KohaneEngine.Scripts.Story.Resolvers
{
    public class ProgressResolver : Resolver
    {
        private readonly KohaneStateManager _stateManager;
        private readonly KohaneBinder _binder;
        private readonly KohaneStoryManager _storyManager;

        public ProgressResolver(KohaneStateManager stateManager, KohaneBinder binder, KohaneStoryManager storyManager)
        {
            _stateManager = stateManager;
            _binder = binder;
            _storyManager = storyManager;
            Functions.Add("selection", Selection);
            Functions.Add("jumpToScene", JumpToScene);
        }

        [StoryFunctionAttr("selection")]
        private ResolveResult Selection(Block block)
        {
            _ = WaitForSelection(block);
            return ResolveResult.SuccessResult();
        }

        [StoryFunctionAttr("jumpToScene")]
        private ResolveResult JumpToScene(Block arg)
        {
            _storyManager.JumpToScene(arg.GetArg<string>(0));
            return ResolveResult.SuccessResult();
        }

        private async Task WaitForSelection(Block block)
        {
            _stateManager.SwitchState(KohaneState.ResolveEnd);
            _stateManager.AddFlag(KohaneFlag.InSystem);

            var selections = block.GetArg<string>(0).Split('|')
                .Select(s => s.Split(':'))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
            
            List<Task<bool>> tasks = new();
            var selection = "";
            
            foreach(var (k, v) in selections)
            {
                var button = _binder.CreateSelection(k);
                
                var buttonClickedTask = new TaskCompletionSource<bool>();
                tasks.Add(buttonClickedTask.Task);
                button.onClick.AddListener(delegate
                {
                    selection = v;
                    buttonClickedTask.SetResult(true);
                });
            }
            
            _binder.selectionsCanvasGroup.DOFade(1, 1f).OnComplete(() =>
            {
                _binder.selectionsCanvasGroup.blocksRaycasts = true;
            });
            await Task.WhenAny(tasks);
            _binder.selectionsCanvasGroup.blocksRaycasts = false;
            _binder.selectionsCanvasGroup.DOFade(0, 1f).OnComplete(() =>
            {
                _stateManager.RemoveFlag(KohaneFlag.InSystem);
                _stateManager.SwitchState(KohaneState.Ready);
                _storyManager.JumpToScene(selection, fromSelection: true);
                _storyManager.ResolveNext();
            });
        }
    }
}