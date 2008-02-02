using System;
using System.Collections.Generic;

namespace RuleKnit
{
	public static class CollectionUtility
	{
		public static bool AreEqual<TKey, TValue>(IDictionary<TKey, TValue> dictA, IDictionary<TKey, TValue> dictB)
		{
			if(dictA.Count != dictB.Count)
			{
				return false;
			}
			foreach(TKey key in dictA.Keys)
			{
				if (!dictA.ContainsKey(key)) return false;
				TValue aValue = dictA[key];
				TValue bValue = dictB[key];
				if (aValue == null)
				{
					return bValue == null;
				}
				if (!aValue.Equals(bValue))
				{
					return false;
				}
			}
			return true;
		}
	}
}
