using System;
using KohaneEngine.Scripts.Framework;
using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.Story
{
    public class KohaneStoryManager
    {
        private static StoryResolver StoryResolver => KohaneEngine.StoryResolver;
        
        private int _currentSceneIndex = 0;
        private int _currentBlockIndex = 0;
        private KohaneStruct _story;
        
        private KohaneStateManager _stateManager;
        
        private Scene CurrentScene => _story.scenes[_currentSceneIndex];
        private Block CurrentBlock => _story.scenes[_currentSceneIndex].blocks[_currentBlockIndex];
        
        public KohaneStoryManager(KohaneStateManager stateManager)
        {
            _stateManager = stateManager;
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
            while (!_stateManager.IsInState(KohaneState.WaitingForClick))
            {
                StoryResolver.Resolve(CurrentBlock);
                ToNextBlock();
            }
            _stateManager.SwitchState(KohaneState.Ready);
        }

        private void ToNextBlock()
        {
            // Next block
            if (CurrentScene.blocks.Count > _currentBlockIndex + 1)
            {
                _currentBlockIndex++;
                return;
            }

            // Next scene
            if (_story.scenes.Count > _currentSceneIndex + 1)
            {
                _currentSceneIndex++;
                _currentBlockIndex = 0;
                return;
            }

            throw new Exception("Reach end of story");
        }
    }
}