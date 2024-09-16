using System.Threading.Tasks;
using KohaneEngine.Scripts.Utils;
using UnityEngine;

namespace KohaneEngine.Scripts.ResourceManager
{
    public class ResourceManager : IResourceManager
    {
        public T LoadResource<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public async Task<T> LoadResourceAsync<T>(string path) where T : Object
        {
            Debug.Log($"[ResourceManager] Loading resource {path} async");
            var operation = Resources.LoadAsync<T>(path);
            await TaskEx.WaitUntil(() => operation.isDone);
            return operation.asset as T;
        }
    }
}