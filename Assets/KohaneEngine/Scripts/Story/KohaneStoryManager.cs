using System;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Structure;
using KohaneEngine.Scripts.Utils;

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

            // Next scene
            if (_story.scenes.Count > CurrentSceneIndex + 1)
            {
                CurrentSceneIndex++;
                CurrentBlockIndex = 0;
                return;
            }

            throw new Exception("Reach end of story");
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