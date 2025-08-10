using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace core.Shared.DependencyInjection
{
	public class Container
	{
		private readonly List<Registration> _registrations;
		private readonly Dictionary<string, object> _instances = new Dictionary<string, object>();

		internal Container(List<Registration> registrations)
		{
			_registrations = registrations;
		}

		private object ResolveSingleInstance(Registration registration)
		{
			var assemblyQualifiedName = registration.InstanceType.AssemblyQualifiedName;

			if (_instances.ContainsKey(assemblyQualifiedName))
			{
				return _instances[assemblyQualifiedName];
			}

			var instance = CreateInstance(registration);
			_instances.Add(assemblyQualifiedName, instance);

			return instance;
		}

		private object CreateInstance(Registration registration)
		{
			LogHelper.Log($"Create instance: {registration.InstanceType}");

			if (registration.Factory != null)
				return registration.Factory();

			var constructor = registration.InstanceType.GetConstructors().FirstOrDefault();
			var parameters = constructor.GetParameters().Select(ResolveConstructorParameter).ToArray();

			var instance = Activator.CreateInstance(registration.InstanceType, parameters);
			
			return instance;
		}

		private object ResolveConstructorParameter(ParameterInfo parameterInfo)
		{
			LogHelper.Log($"Resolve constructor parameter: {parameterInfo.ParameterType}");

			return Resolve(parameterInfo.ParameterType);
		}

		private object ResolveArray(Type arrayType)
		{
			var elementType = arrayType.GetElementType();

			LogHelper.Log($"Resolve array: {arrayType}");

			var instances = _registrations
				.Where(r => r.Types.Any(t => t == elementType))
				.Select(r => Resolve(r.InstanceType))
				.ToArray();

			var array = Array.CreateInstance(elementType, instances.Length);
			for (var i = 0; i < instances.Length; i++)
				array.SetValue(instances[i], i);

			return array;
		}

		public object Resolve(Type type)
		{
			if (type.IsArray)
				return ResolveArray(type);

			var matchingRegistration = _registrations.SingleOrDefault(r => r.Types.Any(t => t == type));

			if (matchingRegistration == null)
				throw new Exception("Could not resolve type " + type.AssemblyQualifiedName);

			if (matchingRegistration.SingleInstance)
				return ResolveSingleInstance(matchingRegistration);

			return CreateInstance(matchingRegistration);
		}

		public T Resolve<T>()
		{
			var resolveType = typeof(T);
			return (T) Resolve(resolveType);
		}

		public Type[] GetRegisteredTypes()
		{
			return _registrations.Select(r => r.InstanceType).ToArray();
		}
	}
}