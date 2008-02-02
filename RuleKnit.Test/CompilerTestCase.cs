using System;
using System.CodeDom;
using System.IO;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class CompilerTestCase : RuleKnitTestCase
	{
		[Test]
		public void WriteOutput()
		{
			StringWriter writer = new StringWriter();
			new Compiler(null, writer).Compile(new CodeTypeDeclaration("A"), false);
			AssertCodeContains("class A { }", writer.ToString(), "");
		}

		[Test]
		public void CompileType()
		{
			CodeTypeDeclaration typeDeclaration = new CodeTypeDeclaration("Foo");
			Type fooType = new Compiler("FooNS").Compile(typeDeclaration, false);
			Assert.AreEqual("Foo", fooType.Name);
			Assert.AreEqual("FooNS", fooType.Namespace);
		}

		[Test]
		public void ThrowsExceptionOnCompileFailure()
		{
			Exception exception = null;
			try 
			{
				CodeTypeDeclaration typeDeclaration = new CodeTypeDeclaration("A");
				CodeMemberMethod method1 = new CodeMemberMethod();
				method1.Name = "method1";
				method1.ReturnType = new CodeTypeReference(typeof(string));
				typeDeclaration.Members.Add(method1);
				CodeMemberMethod method2 = new CodeMemberMethod();
				method2.Name = "method2";
				method2.ReturnType = new CodeTypeReference(typeof(string));
				typeDeclaration.Members.Add(method2);
				new Compiler().Compile(typeDeclaration, false);
			}
			catch(Exception ex)
			{
				exception = ex;
			}
			AssertCodeContains(@"public class A { private string method1() { } private string method2() { } }", exception.Message, "");
			AssertCodeContains(@"'A.method1()': not all code paths return a value", exception.Message, "");
			AssertCodeContains(@"'A.method2()': not all code paths return a value", exception.Message, "");
		}
	}
}
