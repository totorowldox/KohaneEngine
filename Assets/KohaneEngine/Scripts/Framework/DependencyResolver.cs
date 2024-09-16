using System;
using System.Collections.Generic;
using System.Linq;

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
            _typeMap[typeof(TInterface)] = typeof(TImplementation);
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
