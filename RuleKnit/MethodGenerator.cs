using System;
using System.Reflection;
using System.Reflection.Emit;

namespace RuleKnit
{
	/// <summary>
	/// Source: http://geekswithblogs.net/kraki/archive/2006/03/29/73743.aspx
	/// </summary>
	public class MethodGenerator
	{
		public event EventHandler<CreatingMethodBodyEventArgs> CreatingMethodBody;
		public MethodGenerator()
		{ }

		/// <summary>
		/// Generates a new method with a signature that matches the signature of the delegate T.
		/// </summary>
		/// <typeparam name="T">The type of the delegate to return and use as a method signature.</typeparam>
		/// <typeparam name="K">The type of the object with which the generated method will be associated.</typeparam>
		/// <param name="name">The identifying name of the generated method.</param>
		/// <returns>A delegate of type T which points to the generated method</returns>
		public T Generate<T, K>(string name) where T : class
		{
			MethodInfo method = GetMethod(typeof(T));
			ParameterInfo[] parameterDefinitions = method.GetParameters();
			Type[] parameters = new Type[parameterDefinitions.Length];

			for (int index = 0; index < parameterDefinitions.Length; index++)
			{
				parameters[index] = parameterDefinitions[index].ParameterType;
			}

			return (ConstructDynamicMethod(name, method, typeof(T), parameters, typeof(K)).CreateDelegate(typeof(T)) as T);
		}

		/// <summary>
		/// Generates a new method with a signature that matches the signature of the delegate T.
		/// </summary>
		/// <typeparam name="T">The type of the delegate to return and use as a method signature.</typeparam>
		/// <typeparam name="K">The type of the object with which the generated method will be associated.</typeparam>
		/// <param name="name">The identifying name of the generated method.</param>
		/// <param name="instance">The instance of type K to which the method will be bound.</param>
		/// <returns>A delegate of type T which points to the generated method</returns>
		public T Generate<T, K>(string name, K instance) where T : class
		{
			MethodInfo method = GetMethod(typeof(T));
			ParameterInfo[] parameterDefinitions = method.GetParameters();
			Type[] parameters = new Type[parameterDefinitions.Length + 1];

			parameters[0] = typeof(K);
			for (int index = 0; index < parameterDefinitions.Length; index++)
			{
				parameters[index + 1] = parameterDefinitions[index].ParameterType;
			}

			return (ConstructDynamicMethod(name, method, typeof(T), parameters, typeof(K)).CreateDelegate(typeof(T), instance) as T);
		}

		private DynamicMethod ConstructDynamicMethod(string name, MethodInfo method, Type delegateType, Type[] parameters, Type ownerType)
		{
			DynamicMethod dm = new DynamicMethod(name, method.ReturnType, parameters, ownerType);
			ILGenerator il = dm.GetILGenerator();

			if (CreatingMethodBody != null)
			{
				CreatingMethodBody(this, new CreatingMethodBodyEventArgs(name, delegateType, ownerType, il));
			}
			else
			{
				il.Emit(OpCodes.Ret);
			}

			return dm;
		}

		private MethodInfo GetMethod(Type delegateType)
		{
			if (!delegateType.IsSubclassOf(typeof(Delegate)))
			{
				throw new ArgumentException("Type T must be a delegate", "T");
			}
			return delegateType.GetMethod("Invoke");
		}
	}

	public class CreatingMethodBodyEventArgs : EventArgs
	{
		private string _name;
		private Type _delegateType;
		private Type _ownerType;
		private ILGenerator _generator;

		public Type DelegateType
		{
			get { return _delegateType; }
		}

		public ILGenerator ILGenerator
		{
			get { return _generator; }
		}

		public Type OwnerType
		{
			get { return _ownerType; }
		}

		public string Name
		{
			get { return _name; }
		}

		public CreatingMethodBodyEventArgs(string name, Type delegateType, Type ownerType, ILGenerator ilGenerator)
		{
			_name = name;
			_generator = ilGenerator;
			_delegateType = delegateType;
			_ownerType = ownerType;
		}
	}
}
