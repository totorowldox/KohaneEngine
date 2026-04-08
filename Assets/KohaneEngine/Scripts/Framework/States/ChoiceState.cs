using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KohaneEngine.Scripts.Story;

namespace KohaneEngine.Scripts.Framework.States
{
    public class ChoiceState : KohaneState
    {
        private readonly KohaneStoryManager _storyManager;
        private readonly KohaneBinder _binder;
        private string _selectedScene = null;
        private readonly Dictionary<string, string> _selections;

        public ChoiceState(KohaneStateManager stateManager, Dictionary<string, string> selections) : base(stateManager)
        {
            _storyManager = KohaneEngine.Resolver.Resolve<KohaneStoryManager>();
            _binder = KohaneEngine.Resolver.Resolve<KohaneBinder>();
            _selections = selections;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _selectedScene = null;
            StateManager.SetPlayback(PlaybackMode.Normal);
            _ = ShowAndWaitSelection();
        }

        private async UniTask ShowAndWaitSelection()
        {
            _binder.selectionsCanvasGroup.DOFade(1, 0.6f).OnComplete(() =>
            {
                _binder.selectionsCanvasGroup.blocksRaycasts = true;
            });

            _selectedScene = await WaitForPlayerChoice(_selections);

            _binder.selectionsCanvasGroup.blocksRaycasts = false;
            await _binder.selectionsCanvasGroup.DOFade(0, 0.6f);

            if (!string.IsNullOrEmpty(_selectedScene))
            {
                _storyManager.JumpToScene(_selectedScene, fromSelection: true);
            }

            StateManager.TransitionTo<ResolvingState>();
        }

        protected override void HandleInput()
        {
            // Ignore other user inputs
        }

        private async UniTask<string> WaitForPlayerChoice(Dictionary<string, string> selections)
        {
            var tcs = new UniTaskCompletionSource<string>();

            foreach (var (text, scene) in selections)
            {
                var button = _binder.CreateSelection(text);
                button.onClick.AddListener(() => { tcs.TrySetResult(scene); });
            }

            return await tcs.Task;
        }

        public override void OnExit()
        {
            _selections.Clear();
            _selectedScene = null;
            base.OnExit();
        }
    }
}