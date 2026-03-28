using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectH.Core
{
    public class ObjectResolver
    {
        private readonly HashSet<Type> _registrations = new();
        private readonly Dictionary<Type, object> _cachedInstances = new();

        public void Register<T>()
        {
            _registrations.Add(typeof(T));
        }
        
        public void RegisterInstance<T>(T instance)
        {
            _cachedInstances[typeof(T)] = instance;
            _registrations.Add(instance.GetType());
        }
        
        public void Unregister<T>() 
        {
            _registrations.Remove(typeof(T));
            _cachedInstances.Remove(typeof(T));
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public bool TryResolve<T>(out T instance)
        {
            try
            {
                instance = Resolve<T>();
                return instance != null;
            }
            catch (Exception)
            {
                instance = default;
                return false;
            }
        }

        private object Resolve(Type type)
        {
            if (_cachedInstances.TryGetValue(type, out var instance))
                return instance;

            if (!_registrations.Contains(type))
            {
                throw new InvalidOperationException($"No registration for {type.FullName}");
            }

            var constructor = type.GetConstructors().First();
            var args = constructor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
            instance = Activator.CreateInstance(type, args);

            _cachedInstances[type] = instance;
            return instance;
        }

        public void Clear()
        {
            _registrations.Clear();
            _cachedInstances.Clear();
        }
    }
}
