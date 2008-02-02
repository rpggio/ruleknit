using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace RuleKnit
{
	public static class ObjectUtility
	{
		public static string ToString(object o)
		{
			if (o == null)
			{
				return "null";
			}
			Type type = o.GetType();
			if (type == typeof(string))
			{
				return "\"" + o + "\"";
			}
			if (type == typeof(DateTime))
			{
				return string.Format("DateTime.Parse(\"{0}\")", o);
			}
			if (type == typeof(decimal))
			{
				return o + "m";
			}
			if (type == typeof(float))
			{
				return o + "m";
			}
			if (o is ICollection)
			{
				var itemStrings = new List<string>();
				Type itemType = null;
				foreach (var item in (ICollection)o)
				{
					if (itemType == null) itemType = item.GetType();
					itemStrings.Add(ToString(item));
				}
				return string.Format("new {0}[] {{\n\t{1}\n\t}}", itemType.Name, string.Join(",\n\t", itemStrings.ToArray()));
			}
			return o.ToString();
		}

		public static string ToStringAsNew(object o)
		{
			Type type = o.GetType();
			ConstructorInfo ctorInfo = null;
			foreach(var ci in type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				if (ctorInfo == null || ci.GetParameters().Length > ctorInfo.GetParameters().Length)
				{
					ctorInfo = ci;
				}
			}
			var paramStrings = new List<string>();
			var propStrings = new List<string>();
			foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				string propertyNameLower = property.Name.ToLower();
				string value = ToString(property.GetValue(o, null));
				if (ctorInfo != null)
				{
					var parameterInfo = ctorInfo.GetParameters()
						.SingleOrDefault(pi => propertyNameLower == pi.Name.ToLower());
					if (parameterInfo != null)
					{
						paramStrings.Add(
							string.Format("/*{0}*/ {1}", parameterInfo.Name, value));
						continue;
					}
				}
				propStrings.Add(
					string.Format("{0}={1}", property.Name, value));
			}
			string result = string.Format("new {0}({1})",
				type.Name,
				String.Join(", ", paramStrings.ToArray()));
			if (propStrings.Count > 0) 
			{
				result += string.Format(" {{ {0} }}", String.Join(" ", propStrings.ToArray()));
			}
			return result;
		}
	}
}