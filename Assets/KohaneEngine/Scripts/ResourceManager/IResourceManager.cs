using System.Threading.Tasks;
using UnityEngine;

namespace KohaneEngine.Scripts.ResourceManager
{
    // TODO: implement deferred loading
    public interface IResourceManager
    {
        /// <summary>
        /// Load a resource synchronously
        /// </summary>
        /// <param name="path">Path to the resource</param>
        /// <typeparam name="T">Resource type</typeparam>
        /// <returns></returns>
        public T LoadResource<T>(string path) where T : Object;
        
        /// <summary>
        /// Load a resource asynchronously
        /// </summary>
        /// <param name="path">Path to the resource</param>
        /// <typeparam name="T">Resource type</typeparam>
        /// <returns></returns>
        public Task<T> LoadResourceAsync<T>(string path) where T : Object;
    }
}