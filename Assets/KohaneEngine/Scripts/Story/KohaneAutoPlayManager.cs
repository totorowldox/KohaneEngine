using KohaneEngine.Scripts.Framework;
using UnityEngine;

namespace KohaneEngine.Scripts.Story
{
    public class KohaneAutoPlayManager
    {
        private readonly KohaneStateManager _stateManager;

        public KohaneAutoPlayManager(KohaneStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        public void ToggleAutoPlay()
        {
            if (_stateManager.HasFlag(KohaneFlag.Auto))
            {
                Debug.Log("<color=#89c3eb>[AutoPlayManager]</color> Ending auto play");
                _stateManager.RemoveFlag(KohaneFlag.Auto);
            }
            else
            {
                Debug.Log("<color=#89c3eb>[AutoPlayManager]</color> Starting auto play");
                _stateManager.AddFlag(KohaneFlag.Auto);
            }
        }
    }
}