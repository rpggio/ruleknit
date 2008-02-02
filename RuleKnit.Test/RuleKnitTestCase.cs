using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using NUnit.Framework;

namespace RuleKnit.Test
{
	public abstract class RuleKnitTestCase
	{
		protected static string CleanCode(string code)
		{
			code = Regex.Replace(code, @"//.*\r\n", "", RegexOptions.Multiline);
			code = Regex.Replace(code, @"\s{1,}", "", RegexOptions.Multiline);
			return code;
		}

		protected void AssertCodeIsEqual(string expected, string result, string message)
		{
			Assert.AreEqual(CleanCode(expected), CleanCode(result), message);
		}

		protected void AssertCodeContains(string match, string code, string message)
		{
			if (!CleanCode(code).Contains(CleanCode(match)))
			{
				Assert.AreEqual(match, code, message);
			}
		}

		protected static string Generate(CodeObject codeObject)
		{
			StringWriter writer = new StringWriter();
			CodeGeneratorOptions generatorOptions = new CodeGeneratorOptions();
			generatorOptions.IndentString = "\t";
			CSharpCodeProvider codeProvider = new CSharpCodeProvider();
			if (codeObject is CodeTypeDeclaration)
			{
				codeProvider.GenerateCodeFromType((CodeTypeDeclaration)codeObject, writer, generatorOptions);
			}
			else if (codeObject is CodeExpression)
			{
				codeProvider.GenerateCodeFromExpression((CodeExpression)codeObject, writer, generatorOptions);
			}
			else if (codeObject is CodeTypeMember)
			{
				codeProvider.GenerateCodeFromMember((CodeTypeMember)codeObject, writer, generatorOptions);
			}
			else if (codeObject is CodeStatement)
			{
				codeProvider.GenerateCodeFromStatement((CodeStatement)codeObject, writer, generatorOptions);
			}
			else
			{
				throw new ArgumentException("CodeObject type not supported: " + codeObject);
			}
			return writer.ToString();
		}
	}
}
