using System;
using System.Collections.Generic;
using KohaneEngine.Scripts.Story;
using KohaneEngine.Scripts.Structure;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    public class StoryResolver
    {
        private readonly Dictionary<string, Type> _typeMap = new();

        /// <summary>
        /// Register a resolver to a block type
        /// </summary>
        /// <typeparam name="T">Resolver</typeparam>
        public void Register<T>(string type)
        {
            if (!_typeMap.TryAdd(type, typeof(T)))
            {
                throw new InvalidOperationException($"Block type {type} has been registered");
            }
        }
        
        /// <summary>
        /// Register a resolver to some block types
        /// </summary>
        /// <typeparam name="T">Resolver</typeparam>
        public void Register<T>(params string[] types)
        {
            foreach (var type in types)
            {
                if (!_typeMap.TryAdd(type, typeof(T)))
                {
                    throw new InvalidOperationException($"Block type {type} has been registered");
                }
            }
        }

        /// <summary>
        /// Resolve a story block
        /// </summary>
        public void Resolve(Block block)
        {
            if (!_typeMap.TryGetValue(block.type, out var resolverType))
            {
                throw new InvalidOperationException($"Unknown block type {block.type}");
            }
            
            Debug.Log($"[StoryResolver] Resolving type {block.type}.");
            
            var resolver = KohaneEngine.Resolver.ResolveByType(resolverType) as Resolver;
            resolver!.Resolve(block);
        }
    }
}