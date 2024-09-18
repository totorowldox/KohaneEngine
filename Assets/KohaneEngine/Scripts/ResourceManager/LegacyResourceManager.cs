using System.Threading.Tasks;
using KohaneEngine.Scripts.Utils;
using UnityEngine;

namespace KohaneEngine.Scripts.ResourceManager
{
    public class LegacyResourceManager : IResourceManager
    {
        public T LoadResource<T>(string path) where T : Object
        {
            path = path.WithoutExtension();
            return Resources.Load<T>(path);
        }

        public async Task<T> LoadResourceAsync<T>(string path) where T : Object
        {
            Debug.Log($"[LegacyResourceManager] Loading resource {path} async");
            path = path.WithoutExtension();
            var operation = Resources.LoadAsync<T>(path);
            await TaskEx.WaitUntil(() => operation.isDone);
            return operation.asset as T;
        }
    }
}