using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;

namespace RuleKnit
{
	public struct Signature 
	{
		private string name;
		private Type type;

		public static Signature Create(PropertyInfo propertyInfo)
		{
			return new Signature(propertyInfo.Name, propertyInfo.PropertyType);
		}

		public static Signature Create(MethodInfo methodInfo)
		{
			return new Signature(methodInfo.Name, methodInfo.ReturnType);
		}

		public Signature(string name, Type type)
		{
			this.name = name;
			this.type = type;
		}

		public string Name
		{
			get { return name; }
		}

		public string NameAsVariable
		{
			get { return Char.ToLower(name[0]) + name.Substring(1, name.Length - 1); }
		}
		
		public Type Type
		{
			get { return type; }
		}
		
		public bool IsImmutable
		{
			get
			{
				return type.IsValueType
				       || typeof(ReadOnlyCollectionBase).IsAssignableFrom(type)
				       || type.GetCustomAttributes(typeof(ImmutableAttribute), false).Length > 0
				       || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>);
			}
		}

		public bool Satisfies(Signature signature)
		{
			return String.Equals(signature.Name, name, StringComparison.OrdinalIgnoreCase)
			       && signature.Type.IsAssignableFrom(type);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Signature))
			{
				return false;
			}
			Signature other = (Signature)obj;
			return String.Equals(other.Name, name, StringComparison.OrdinalIgnoreCase)
			       && type.Equals(other.Type);
		}

		public override int GetHashCode()
		{
			return name.ToLower().GetHashCode() ^ type.GetHashCode();
		}

		public override string ToString()
		{
            return ObjectUtility.ToStringAsNew(this);
		}
	}
}