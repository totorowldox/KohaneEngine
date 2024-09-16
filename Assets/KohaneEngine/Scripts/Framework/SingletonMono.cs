using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    [DisallowMultipleComponent]
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                Debug.LogWarning(typeof(T) + " has been added, destroying the new one.");
                return;
            }

            if (this is not T t) return;
            Instance = t;
            OnAwake();
        }

        /// <summary>
        /// Invoke on Awake
        /// </summary>
        protected virtual void OnAwake()
        {
        }
    }
}