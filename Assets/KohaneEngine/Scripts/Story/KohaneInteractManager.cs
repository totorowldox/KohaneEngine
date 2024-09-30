using KohaneEngine.Scripts.Framework;
using UnityEngine;

namespace KohaneEngine.Scripts.Story
{
    public class KohaneInteractManager : MonoBehaviour
    {
        private KohaneStateManager _stateManager;
        private KohaneStoryManager _storyManager;
        private KohaneAnimator _animator;
        private KohaneAutoPlayManager _autoPlayManager;

        private void Awake()
        {
            _storyManager = KohaneEngine.Resolver.Resolve<KohaneStoryManager>();
            _stateManager = KohaneEngine.Resolver.Resolve<KohaneStateManager>();
            _animator = KohaneEngine.Resolver.Resolve<KohaneAnimator>();
            _autoPlayManager = KohaneEngine.Resolver.Resolve<KohaneAutoPlayManager>();
        }
        
        private void Update()
        {
            // Autoplay manager will take over the next step
            if (DoAutoPlay())
            {
                return;
            }
            
            // If we don't have input, we do not perform next step
            if (!CanPerformNextStep())
            {
                return;
            }

            PerformNextStep();
        }

        public void PerformNextStep()
        {
            // Break autoplay when input is pressed
            if (CheckAutoPlay())
            {
                _autoPlayManager.ToggleAutoPlay();
            }
            // Interrupt animation when input is pressed
            else if (_stateManager.CanSkip() &&_stateManager.HasFlag(KohaneFlag.Animating))
            {
                _animator.InterruptAnimation();
            }
            // Then we can perform next step
            else
            {
                _storyManager.ResolveNext();
            }
        }

        private bool DoAutoPlay()
        {
            // If we're not ready, return
            if (!_stateManager.CanResolve())
            {
                return false;
            }
            // If we're not in auto/skip mode, return
            if (!CheckAutoPlay())
            {
                return false;
            }
            _storyManager.ResolveNext();
            return true;
        }

        private bool CheckAutoPlay() =>
            _stateManager.HasFlag(KohaneFlag.Auto) || _stateManager.HasFlag(KohaneFlag.Skip);

        private static bool CanPerformNextStep() =>
            Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return);
    }
}