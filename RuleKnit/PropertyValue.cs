using System;
using System.CodeDom;
using System.Reflection;

namespace RuleKnit
{
	public class PropertyValue : IRuleElement
	{
		Signature _declaringElementSignature;
		Signature _propertySignature;

		public PropertyValue(Signature declaringElementSignature, Signature propertySignature)
		{
			_declaringElementSignature = declaringElementSignature;
			_propertySignature = propertySignature;
		}

		public Signature Signature
		{
			get { return _propertySignature; } 
		}

		public RuleEvaluation EvaluationMode
		{
			get { return RuleEvaluation.Once; }
		}

		public CodeExpression ToCode()
		{
			return new CodePropertyReferenceExpression(
				new CodeVariableReferenceExpression(_declaringElementSignature.NameAsVariable), _propertySignature.Name);
		}

		public Signature[] Dependencies
		{
			get { return new Signature[] { _declaringElementSignature }; }
		}

		public static IRuleElement[] CreateFromType(Signature declaringElementSignature, Type type)
		{
			if (type.GetCustomAttributes(typeof(ExposePropertiesAttribute), true).Length > 0)
			{
				return Array.ConvertAll<PropertyInfo, PropertyValue>(
					type.GetProperties(BindingFlags.Instance | BindingFlags.Public),
					delegate(PropertyInfo pi)
					{
						return new PropertyValue(declaringElementSignature, Signature.Create(pi));
					});
			}
			return new IRuleElement[0];
		}

		public override bool Equals(object obj)
		{
            var objAsPv = obj as PropertyValue;
            if (objAsPv == null) return false;
            return objAsPv._declaringElementSignature.Equals(_declaringElementSignature)
               && objAsPv._propertySignature.Equals(_propertySignature);
		}

		public override int GetHashCode()
		{
            return _declaringElementSignature.GetHashCode() ^ _propertySignature.GetHashCode();
		}
	}
}
