using System;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.Story
{
    public class KohaneStoryManager
    {
        private static StoryResolver StoryResolver => KohaneEngine.StoryResolver;

        private KohaneStruct _story;
        
        private readonly KohaneStateManager _stateManager;
        private readonly KohaneAnimator _animator;

        private Scene CurrentScene => _story.scenes[CurrentSceneIndex];
        private Block CurrentBlock => _story.scenes[CurrentSceneIndex].blocks[CurrentBlockIndex];

        public int CurrentSceneIndex { get; private set; }

        public int CurrentBlockIndex { get; private set; }

        public KohaneStoryManager(KohaneStateManager stateManager, KohaneAnimator animator)
        {
            _stateManager = stateManager;
            _animator = animator;
        }

        public void StartStory(KohaneStruct story)
        {
            _story = story;
            ResolveNext();
        }

        public void ResolveNext()
        {
            if (!_stateManager.SwitchState(KohaneState.Resolving))
            {
                return;
            }
            
            while (!_stateManager.IsInState(KohaneState.ResolveEnd))
            {
                StoryResolver.Resolve(CurrentBlock);
                ToNextBlock();
            }

            _animator.StartAnimation();
        }

        private void ToNextBlock()
        {
            // Next block
            if (CurrentScene.blocks.Count > CurrentBlockIndex + 1)
            {
                CurrentBlockIndex++;
                return;
            }

            throw new Exception("Reach end of scene");
        }

        public void JumpToScene(string sceneName, bool fromSelection = false)
        {
            CurrentSceneIndex = _story.scenes.FindIndex((x) => x.label == sceneName);
            // TODO: somewhat magic/cheat approach? need to improve!
            CurrentBlockIndex = fromSelection ? 0 : -1;
        }

        public void JumpToLine(int sceneIndex, int blockLine)
        {
            CurrentSceneIndex = sceneIndex;
            while (CurrentBlockIndex < blockLine)
            {
                ResolveNext();
                _animator.InterruptAnimation();
            }
        }
    }
}