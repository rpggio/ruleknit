using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RuleKnit
{
	public class RulesetFactory
	{
		private static RulesetFactory _current;
		private Dictionary<Type, object> _cache = new Dictionary<Type, object>();

		public static RulesetFactory Current
		{
			get
			{
				if (_current == null)
				{
					_current = new RulesetFactory();
				}
				return _current;
			}
		}

		public T Create<T>()
		{
#if DEBUG
			bool treatWarningsAsErrors = true;
#else
			bool treatWarningsAsErrors = false;
#endif
			return Create<T>(new StringWriter(), treatWarningsAsErrors);
		}

		public T Create<T>(TextWriter writer, bool treatWarningsAsErrors)
		{
			Type rulesetType = typeof(T);
			if (_cache.ContainsKey(rulesetType))
			{
				return (T)_cache[rulesetType];
			}
			CodeTypeDeclaration codeTypeDeclaration = new RulesetTypeDefinition(rulesetType).ToCode();
			Compiler compiler = new Compiler(rulesetType.Namespace, writer, GetReferences(rulesetType));
			Type implementationType = compiler.Compile(codeTypeDeclaration, treatWarningsAsErrors);
			T instance = (T)Activator.CreateInstance(implementationType);
			_cache[rulesetType] = instance;
			return instance;
		}

		protected internal static string[] GetReferences(Type type)
		{
			List<string> references = new List<string>();
			references.Add(type.Assembly.Location);
			foreach (AssemblyName name in type.Assembly.GetReferencedAssemblies())
			{
				if (name.Name == "mscorlib")
				{
					continue;
				}
				if (name.Name.StartsWith("System"))
				{
					references.Add(name.Name + ".dll");
				}
				else
				{
					Assembly refAssembly = Assembly.Load(name);
					references.Add(refAssembly.Location);
				}
			}
			return references.ToArray();
		}
	}
}
