using KohaneEngine.Scripts.Framework;
using UnityEngine;

namespace KohaneEngine.Scripts.Story
{
    public class KohaneInputManager : MonoBehaviour
    {
        private KohaneStateManager _stateManager;
        private KohaneStoryManager _storyManager;
        private KohaneAnimator _animator;

        private void Awake()
        {
            _storyManager = KohaneEngine.Resolver.Resolve<KohaneStoryManager>();
            _stateManager = KohaneEngine.Resolver.Resolve<KohaneStateManager>();
            _animator = KohaneEngine.Resolver.Resolve<KohaneAnimator>();
        }
        
        private void Update()
        {
            if (!CanPerformNextStep())
            {
                return;
            }

            if (_stateManager.HasFlag(KohaneFlag.Animating))
            {
                _animator.InterruptAnimation();
            }
            else
            {
                _storyManager.ResolveNext();
            }
        }

        private bool CanPerformNextStep() =>
            Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonUp(0);
    }
}