using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KohaneEngine.Scripts.Framework
{
    public class DependencyResolver
    {
        private readonly Dictionary<Type, Type> _typeMap = new();
        private readonly Dictionary<Type, object> _implementationMap = new();

        /// <summary>
        /// Registers a type and its corresponding concrete type
        /// </summary>
        /// <typeparam name="TInterface">The concrete type</typeparam>
        /// <typeparam name="TImplementation">The implemented type</typeparam>
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            Debug.Log($"<color=#68be8d>[DependencyResolver]</color> Using {typeof(TImplementation)} for {typeof(TInterface)}");
            _typeMap[typeof(TInterface)] = typeof(TImplementation);
        }
        
        /// <summary>
        /// Registers a class for later instantiation
        /// </summary>
        /// <typeparam name="TClass">The class type</typeparam>
        public void Register<TClass>() where TClass : class
        {
            _typeMap[typeof(TClass)] = typeof(TClass);
        }
        
        /// <summary>
        /// Registers an instance of a specific type
        /// </summary>
        /// <typeparam name="T">Type of the instance</typeparam>
        /// <param name="instance">The instance to register</param>
        public void RegisterInstance<T>(T instance)
        {
            Debug.Log($"<color=#68be8d>[DependencyResolver]</color> Using instance of {typeof(T)}");
            _implementationMap[typeof(T)] = instance;
        }

        /// <summary>
        /// Resolves an instance of the specified type
        /// </summary>
        /// <typeparam name="T">Type of the instance</typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object ResolveByType(Type type)
        {
            return Resolve(type);
        }

        private object Resolve(Type type)
        {
            if (!_typeMap.TryGetValue(type, out var implementationType))
            {
                if (!type.IsAbstract)
                {
                    implementationType = type;
                }
                else
                {
                    throw new InvalidOperationException($"No registration for {type}");
                }
            }

            if (_implementationMap.TryGetValue(implementationType, out var implementation))
            {
                return implementation;
            }
            
            var constructor = implementationType.GetConstructors().First();
            var parameters = constructor.GetParameters();
            implementation = parameters.Length == 0
                ? Activator.CreateInstance(implementationType)
                : constructor.Invoke(parameters.Select(parameter => Resolve(parameter.ParameterType)).ToArray());
            _implementationMap.Add(implementationType, implementation);
            
            return implementation;
        }
    }
}
