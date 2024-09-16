using System;
using System.Collections.Generic;
using System.Reflection;
using Codice.Client.Common;
using Codice.CM.SEIDInfo;
using KohaneEngine.Scripts.Story;
using KohaneEngine.Scripts.Structure;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    public class StoryResolver
    {
        private readonly Dictionary<string, Type> _typeMap = new();

        /// <summary>
        /// Register a resolver to a type, TYPE MUST BE ABSTRACT INTERFACE
        /// </summary>
        /// <typeparam name="T">Block type</typeparam>
        public void Register<T>(string type)
        {
            if (!_typeMap.TryAdd(type, typeof(T)))
            {
                throw new InvalidOperationException($"Block type {type} has been registered");
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
            
            Debug.Log($"Resolving type {block.type}.");
            
            var resolver = KohaneEngine.Resolver.ResolveByType(resolverType) as Resolver;
            resolver.Resolve(block);
            
            // Finally process the result, deal with sth like "click"
            
        }
    }
}