using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace RuleKnit
{
	public class Compiler
	{
		private string _compilationNamespace;
		private TextWriter _writer;
		private string[] _references;

		public Compiler()
		: this(null, new string[0])
		{
		}

		public Compiler(string compilationNamespace, params string[] references)
			: this(compilationNamespace, new StringWriter(), references)
		{
		}

		public Compiler(string compilationNamespace, TextWriter writer, params string[] references)
		{
			_compilationNamespace = compilationNamespace;
			_writer = writer;
			_references = references;
		}

		public Type Compile(CodeTypeDeclaration typeDeclaration, bool treatWarningsAsErrors)
		{
			Assembly compiled = Compile(CreateCompileUnit(typeDeclaration), treatWarningsAsErrors);
			string typeName = string.Format("{0}.{1}", _compilationNamespace, typeDeclaration.Name);
			return compiled.GetType(typeName, true);
		}

		public Assembly Compile(CodeCompileUnit compileUnit, bool treatWarningsAsErrors)
		{
			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerParameters compilerParameters = new CompilerParameters();
#if DEBUG
			compilerParameters.TempFiles = new TempFileCollection(
				Environment.GetEnvironmentVariable("TEMP"), true);
			compilerParameters.IncludeDebugInformation = true;
			compilerParameters.TreatWarningsAsErrors = treatWarningsAsErrors;
#else
			compilerParameters.GenerateInMemory = true;
#endif
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			options.IndentString = "\t";
			provider.GenerateCodeFromCompileUnit(compileUnit, _writer, options);
			CompilerResults cr = provider.CompileAssemblyFromDom(
				compilerParameters, compileUnit);
			if (cr.Errors.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				new CSharpCodeProvider().GenerateCodeFromCompileUnit(compileUnit,
																	  new StringWriter(stringBuilder), options);
				foreach (CompilerError compilerError in cr.Errors)
				{
					stringBuilder.AppendLine(compilerError.ToString());
				}
				throw new Exception(stringBuilder.ToString());
			}
			return cr.CompiledAssembly;
		}

		protected CodeCompileUnit CreateCompileUnit(CodeTypeDeclaration typeDeclaration)
		{
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			CodeNamespace codeNamespace = new CodeNamespace(_compilationNamespace);
			codeNamespace.Types.Add(typeDeclaration);
			codeCompileUnit.Namespaces.Add(codeNamespace);
			Array.ForEach(_references, 
							  delegate(string r) { codeCompileUnit.ReferencedAssemblies.Add(r); });
			return codeCompileUnit;
		}
	}
}
