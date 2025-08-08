using System;
using System.Collections.Generic;
using System.Reflection;

namespace core.Shared
{
    public enum ServiceLifetime
    {
        Singleton,
        Transient,
        Scoped
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public Type InterfaceType { get; set; }
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        public ServiceAttribute() { }

        public ServiceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }

        public ServiceAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        public ServiceAttribute(Type interfaceType, ServiceLifetime lifetime)
        {
            InterfaceType = interfaceType;
            Lifetime = lifetime;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ScopedAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TransientAttribute : Attribute
    {
    }

    public interface IServiceScope : IDisposable
    {
        T Get<T>() where T : class;
        object Get(Type serviceType);
    }

    internal class ServiceRegistration
    {
        public Type InterfaceType { get; set; }
        public Type ImplementationType { get; set; }
        public ServiceLifetime Lifetime { get; set; }
        public Func<IServiceScope, object> Factory { get; set; }
        public object SingletonInstance { get; set; }
    }

    public class ServiceScope : IServiceScope
    {
        private readonly Dictionary<Type, ServiceRegistration> _registrations;
        private readonly Dictionary<Type, object> _scopedInstances;
        private bool _disposed;

        internal ServiceScope(Dictionary<Type, ServiceRegistration> registrations)
        {
            _registrations = registrations;
            _scopedInstances = new Dictionary<Type, object>();
        }

        public T Get<T>() where T : class
        {
            return (T)Get(typeof(T));
        }

        public object Get(Type serviceType)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ServiceScope));

            if (!_registrations.TryGetValue(serviceType, out var registration))
                throw new Exception($"Service {serviceType.Name} not registered.");

            return CreateInstance(registration);
        }

        private object CreateInstance(ServiceRegistration registration)
        {
            switch (registration.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    if (registration.SingletonInstance == null)
                    {
                        registration.SingletonInstance = registration.Factory?.Invoke(this) ?? 
                                                       CreateInstanceWithDependencyInjection(registration.ImplementationType);
                    }
                    return registration.SingletonInstance;

                case ServiceLifetime.Scoped:
                    if (!_scopedInstances.TryGetValue(registration.InterfaceType, out var scopedInstance))
                    {
                        scopedInstance = registration.Factory?.Invoke(this) ?? 
                                       CreateInstanceWithDependencyInjection(registration.ImplementationType);
                        _scopedInstances[registration.InterfaceType] = scopedInstance;
                    }
                    return scopedInstance;

                case ServiceLifetime.Transient:
                default:
                    return registration.Factory?.Invoke(this) ?? 
                           CreateInstanceWithDependencyInjection(registration.ImplementationType);
            }
        }

        private object CreateInstanceWithDependencyInjection(Type implementationType)
        {
            var constructors = implementationType.GetConstructors();
            var constructor = constructors[0];

            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(implementationType);
            }

            var args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = Get(parameters[i].ParameterType);
            }

            return Activator.CreateInstance(implementationType, args);
        }

        public void Dispose()
        {
            if (_disposed) return;

            // Dispose de todas as instâncias scoped que implementam IDisposable
            foreach (var instance in _scopedInstances.Values)
            {
                if (instance is IDisposable disposable)
                    disposable.Dispose();
            }

            _scopedInstances.Clear();
            _disposed = true;
        }
    }

    public static class Application
    {
        private static Dictionary<Type, ServiceRegistration> _registrations;
        private static bool _built;

        public static void StartRegistration()
        {
            _registrations = new Dictionary<Type, ServiceRegistration>();
            _built = false;
        }

        #region Manual Registration Methods

        public static void RegisterSingleton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface, new()
        {
            RegisterService<TInterface, TImplementation>(ServiceLifetime.Singleton);
        }

        public static void RegisterScoped<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface, new()
        {
            RegisterService<TInterface, TImplementation>(ServiceLifetime.Scoped);
        }

        public static void RegisterTransient<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface, new()
        {
            RegisterService<TInterface, TImplementation>(ServiceLifetime.Transient);
        }

        public static void RegisterService<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TInterface : class
            where TImplementation : class, TInterface, new()
        {
            if (_built)
                throw new Exception("Service registration is already built");

            var interfaceType = typeof(TInterface);
            if (_registrations.ContainsKey(interfaceType))
                throw new Exception($"Service {interfaceType.Name} already registered.");

            _registrations[interfaceType] = new ServiceRegistration
            {
                InterfaceType = interfaceType,
                ImplementationType = typeof(TImplementation),
                Lifetime = lifetime
            };
        }

        public static void RegisterFactory<TInterface>(Func<IServiceScope, TInterface> factory, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TInterface : class
        {
            if (_built)
                throw new Exception("Service registration is already built");

            var interfaceType = typeof(TInterface);
            if (_registrations.ContainsKey(interfaceType))
                throw new Exception($"Service {interfaceType.Name} already registered.");

            _registrations[interfaceType] = new ServiceRegistration
            {
                InterfaceType = interfaceType,
                Lifetime = lifetime,
                Factory = scope => factory(scope)
            };
        }

        public static void RegisterInstance<TInterface>(TInterface instance)
            where TInterface : class
        {
            if (_built)
                throw new Exception("Service registration is already built");

            var interfaceType = typeof(TInterface);
            if (_registrations.ContainsKey(interfaceType))
                throw new Exception($"Service {interfaceType.Name} already registered.");

            _registrations[interfaceType] = new ServiceRegistration
            {
                InterfaceType = interfaceType,
                Lifetime = ServiceLifetime.Singleton,
                SingletonInstance = instance
            };
        }

        #endregion

        #region Auto Registration

        public static void AutoRegisterServices()
        {
            if (_built)
                throw new Exception("Application is already built");

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                var serviceAttribute = type.GetCustomAttribute<ServiceAttribute>();
                if (serviceAttribute != null)
                {
                    var interfaceType = serviceAttribute.InterfaceType ?? type;
                    var lifetime = DetermineLifetime(type, serviceAttribute);

                    if (_registrations.ContainsKey(interfaceType))
                        continue;

                    _registrations[interfaceType] = new ServiceRegistration
                    {
                        InterfaceType = interfaceType,
                        ImplementationType = type,
                        Lifetime = lifetime
                    };
                }
            }
        }

        private static ServiceLifetime DetermineLifetime(Type type, ServiceAttribute serviceAttribute)
        {
            if (serviceAttribute.Lifetime != ServiceLifetime.Transient)
                return serviceAttribute.Lifetime;

            if (type.GetCustomAttribute<SingletonAttribute>() != null)
                return ServiceLifetime.Singleton;

            if (type.GetCustomAttribute<ScopedAttribute>() != null)
                return ServiceLifetime.Scoped;

            if (type.GetCustomAttribute<TransientAttribute>() != null)
                return ServiceLifetime.Transient;

            return ServiceLifetime.Transient;
        }

        #endregion

        #region Service Resolution

        public static void Build()
        {
            if (_built) return;
            _built = true;
        }

        public static IServiceScope CreateScope()
        {
            if (!_built)
                throw new Exception("Application not built. Call Build() first.");

            return new ServiceScope(_registrations);
        }

        public static T Get<T>() where T : class
        {
            using (var scope = CreateScope())
            {
                return scope.Get<T>();
            }
        }

        public static void WithScope(Action<IServiceScope> action)
        {
            using (var scope = CreateScope())
            {
                action(scope);
            }
        }

        public static TResult WithScope<TResult>(Func<IServiceScope, TResult> func)
        {
            using (var scope = CreateScope())
            {
                return func(scope);
            }
        }

        #endregion

        public static void Reset()
        {
            _registrations?.Clear();
            _built = false;
        }
    }
}
