using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KohaneEngine.Scripts.Story;
using KohaneEngine.Scripts.Structure;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    public class StoryResolver
    {
        private readonly Dictionary<string, Type> _typeMap = new();

        /// <summary>
        /// Register all functions of StoryFunction attribute under the namespace
        /// </summary>
        /// <param name="namespaceName">The given namespace</param>
        public void RegisterAllOf(string namespaceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var namespaceTypes = types.Where(t => t.Namespace == namespaceName);
            foreach (var type in namespaceTypes)
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                              BindingFlags.Instance | BindingFlags.Static |
                                              BindingFlags.DeclaredOnly);
                foreach (var method in methods.Where(x => x.IsDefined(typeof(StoryFunctionAttr), false)))
                {
                    var attribute = method.GetCustomAttribute(typeof(StoryFunctionAttr));
                    if (attribute is StoryFunctionAttr customAttribute)
                    {
                        Console.WriteLine($"    Description: {customAttribute.Description}");
                        if (!_typeMap.TryAdd(customAttribute.Name, method.DeclaringType))
                        {
                            throw new InvalidOperationException($"Block type {type} has been registered");
                        }
                    }
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
            
            var resolver = KohaneEngine.Resolver.ResolveByType(resolverType) as Resolver;
            var ret = resolver!.Resolve(block);
            if (!ret.Success)
            {
                Debug.LogError(ret.Reason);
            }
        }
    }
}