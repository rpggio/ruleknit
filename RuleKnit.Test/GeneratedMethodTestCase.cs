using System;
using System.Reflection;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class GeneratedMethodTestCase : RuleKnitTestCase
	{
		[Test]
		public void ToCode()
		{
			GeneratedMethod method = new GeneratedMethod(typeof(AdditionTesting).GetMethod("AddNumbers"));
			string expected = @"public override int AddNumbers(float number3){
													int add;
													int number1 = Number1();
													string number2 = Number2(number1);
													add = Add(number1, number2, number3);
													return add;
												}";
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(AdditionTesting)));
			AssertCodeIsEqual(expected, Generate(method.ToCode(list)), "");
		}

		[Test]
		public void ToCodeProtected()
		{
			GeneratedMethod method = new GeneratedMethod(
				typeof(AdditionTesting).GetMethod("AddNumbersProtected", BindingFlags.NonPublic | BindingFlags.Instance));
			string expected = "protected override int AddNumbersProtected(float number3){";
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(AdditionTesting)));
			AssertCodeContains(expected, Generate(method.ToCode(list)), "");
		}

		[Test]
		public void ToCodeIteration()
		{
			GeneratedMethod method = new GeneratedMethod(typeof(AdditionTesting).GetMethod("AddNumbersIterative"));
			string expected = @"public override int AddNumbersIterative(float number3, int iteration, int iterations, int add){
													bool finished;
													do {
														int number1 = Number1();
														string number2 = Number2(number1);
														finished = Finished(iterations, iteration);
														add = Add(number1, number2, number3);
														iteration = Iteration(iteration);
													}
													while(!finished);
													return add;
												}";
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(AdditionTesting)));
			AssertCodeIsEqual(expected, Generate(method.ToCode(list)), "");
		}

		[Test]
		public void ToCodeIterationIndentation()
		{
			GeneratedMethod method = new GeneratedMethod(typeof(AdditionTesting).GetMethod("AddNumbersIterative"));
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(AdditionTesting)));
			string code = Generate(method.ToCode(list));
			Assert.IsTrue(code.Contains("\npublic override int AddNumbersIterative"));
			Assert.IsTrue(code.Contains("\n\tbool finished;"));
			Assert.IsTrue(code.Contains("\n\tdo {"));
			// can't figure out how to override the default indent level
			//Assert.IsTrue(code.Contains("\n\t\tint number1"));
			//Assert.IsTrue(code.Contains("Iteration(iteration);\r\n\t}"));
			Assert.IsTrue(code.Contains("\n\twhile(!finished);"));
			Assert.IsTrue(code.Contains("\n\treturn add;"));
		}

		[Test]
		public void ToCodeIterationReturnValueDeclaration()
		{
			GeneratedMethod method = new GeneratedMethod(typeof(SimpleTestingClass).GetMethod("RunB"));
			string expected = @"public override int RunB(){
													int b;
													bool finished;
													do {
														int a = A();
														b = B(a);
														finished = Finished();
													}
													while(!finished);
													return b;
												}";
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(SimpleTestingClass)));
			AssertCodeIsEqual(expected, Generate(method.ToCode(list)), "");
		}

		[Test]
		public void ToCodeIterationWithInvariant()
		{
			GeneratedMethod method = new GeneratedMethod(typeof(AdditionTesting).GetMethod("AddNumbersIterativeWithInvariant"));
			string expected = @"public override int AddNumbersIterativeWithInvariant(int addWithInvariant){
													bool immediateFinish;
													float pi = Pi();
													do {
														int number1 = Number1();
														immediateFinish = ImmediateFinish();
														addWithInvariant = AddWithInvariant(number1, pi);
													}
													while(!immediateFinish);
													return addWithInvariant;
												}";
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(AdditionTesting)));
			AssertCodeIsEqual(expected, Generate(method.ToCode(list)), "");
		}

		[Test]
		public void ToCodeWithProperties()
		{
			GeneratedMethod method = new GeneratedMethod(typeof(StringTesting).GetMethod("GetInformalName"));
			string expected = @"public override string GetInformalName(
													RuleKnit.Test.GeneratedMethodTestCase.StringTesting.Person person){
													string informalName;
													string firstName = person.FirstName;
													string lastName = person.LastName;
													string nickname = Nickname(firstName);
													informalName = InformalName(nickname, lastName);
													return informalName;
												}";
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(StringTesting)));
			string result = Generate(method.ToCode(list));
			AssertCodeIsEqual(expected, result , "");
		}

		[Test]
		public void ToCodeSelfRecursion()
		{
			GeneratedMethod method = new GeneratedMethod(typeof(SelfRecursionTesting).GetMethod("Run"));
			string expected = @"public override int Run(int recursive){
													int result;
													bool finished;
													do {
														result = Result(recursive);
														finished = Finished(recursive);
														recursive = Recursive(recursive);
													}
													while(!finished);
													return result;
												}";
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(SelfRecursionTesting)));
			AssertCodeIsEqual(expected, Generate(method.ToCode(list)), "");
		}

		[Test]
		public void ToCodePairRecursionEvaluateLate()
		{
			GeneratedMethod method = new GeneratedMethod(typeof(PairRecursionTestingEvaluateLate).GetMethod("Run"));
			string expected = @"public override int Run(int recursiveA){
													int result;
													bool finished;
													do {
														int recursiveB = RecursiveB(recursiveA);
														result = Result(recursiveA, recursiveB);
														finished = Finished(recursiveB);
														recursiveA = RecursiveA(recursiveB);
													}
													while(!finished);
													return result;
												}";
			RuleElementList list = new RuleElementList(RuleMethod.CreateFromType(typeof(PairRecursionTestingEvaluateLate)));
			AssertCodeIsEqual(expected, Generate(method.ToCode(list)), "");
		}

		abstract class SimpleTestingClass
		{
			[Generated("B", IterationExitRule = "Finished")]
			public abstract int RunB();

			protected static int A()
			{
				throw new NotImplementedException();
			}

			protected static int B(int a)
			{
				throw new NotImplementedException();
			}

			protected static bool Finished()
			{
				throw new NotImplementedException();
			}
		}

		abstract class SelfRecursionTesting
		{
			[Generated("Result", IterationExitRule = "Finished")]
			public abstract int Run(int recursive);

			protected static bool Finished(int recursive)
			{
				throw new NotImplementedException();
			}

			protected static int Recursive(int recursive)
			{
				throw new NotImplementedException();
			}

			protected static int Result(int recursive)
			{
				throw new NotImplementedException();
			}
		}

		abstract class PairRecursionTestingEvaluateLate
		{
			[Generated("Result", IterationExitRule = "Finished")]
			public abstract int Run(int recursiveA);

			protected static bool Finished(int recursiveB)
			{
				throw new NotImplementedException();
			}

			protected static int RecursiveA(int recursiveB)
			{
				throw new NotImplementedException();
			}

			protected static int RecursiveB(int recursiveA)
			{
				throw new NotImplementedException();
			}

			protected static int Result(int recursiveA, int recursiveB)
			{
				throw new NotImplementedException();
			}
		}

		abstract class AdditionTesting
		{
			[Generated("Add")]
			public abstract int AddNumbers(float number3);

			[Generated("Add")]
			protected abstract int AddNumbersProtected(float number3);

			[Generated("Add", IterationExitRule = "Finished")]
			public abstract int AddNumbersIterative(float number3, int iteration, int iterations, int add);

			[Generated("AddWithInvariant", IterationExitRule = "ImmediateFinish")]
			public abstract int AddNumbersIterativeWithInvariant(int addWithInvariant);

			protected static bool Finished(int iterations, int iteration)
			{
				throw new NotImplementedException();
			}

			protected static int Iteration(int iteration)
			{
				throw new NotImplementedException();
			}

			protected static int Number1() 
			{ 
				throw new NotImplementedException(); 
			}
			
			protected static string Number2(int number1) 
			{ 
				throw new NotImplementedException(); 
			}

			protected static int Add(int number1, string number2, float number3)
			{
				throw new NotImplementedException();
			}

			protected static int AddWithInvariant(int number1, float pi)
			{
				throw new NotImplementedException();
			}

			protected static bool ImmediateFinish()
			{
				return true;
			}

			[Evaluation(RuleEvaluation.Once)]
			protected static float Pi()
			{
				throw new NotImplementedException();
			}
		}

		abstract class StringTesting
		{
			[Generated("InformalName")]
			public abstract string GetInformalName(Person person);

			protected static string Nickname(string firstName)
			{
				throw new NotImplementedException();
			}

			protected static string InformalName(string nickname, string lastName)
			{
				throw new NotImplementedException();
			}

			[ExposeProperties]
			public class Person{
				private string firstName;
				private string lastName;

				public Person(string firstName, string lastName)
				{
					this.firstName = firstName;
					this.lastName = lastName;
				}

				public string FirstName
				{
					get { return firstName; }
				}

				public string LastName
				{
					get { return lastName; }
				}
			}
		}
	}
}
