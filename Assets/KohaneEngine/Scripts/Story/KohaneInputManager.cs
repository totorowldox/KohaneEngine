using System;
using UnityEngine;

namespace KohaneEngine.Scripts.Story
{
    public class KohaneInputManager : MonoBehaviour
    {
        private KohaneStoryManager _storyManager;

        private void Awake()
        {
            _storyManager = KohaneEngine.Resolver.Resolve<KohaneStoryManager>();
        }
        
        private void Update()
        {
            if (NextStep)
            {
                _storyManager.ResolveNext();
            }
        }

        private bool NextStep =>
            Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonUp(0);
    }
}