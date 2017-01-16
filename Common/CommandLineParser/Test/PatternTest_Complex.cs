using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Xsd2Db.CommandLineParser.Test
{
	/// <summary>
	/// Summary description for ParserTest.
	/// </summary>
	public abstract class PatternTest_Complex
	{
		/// <summary>
		/// The parameter regex to use
		/// </summary>
		public readonly Regex Regex;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="regex">The parser to use</param>
		public PatternTest_Complex(Regex regex)
		{
			this.Regex = regex;
		}

		/// <summary>
		/// Match a command line having no arguments
		/// </summary>
		[Test]
		public void MatchProgramWithNoArguments()
		{
			string commandline = HelperFunction.MakeCommandLine(String.Empty);
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 0);
		}

		/// <summary>
		/// Match a command line having a single switch specified with the "-"
		/// delimiter.
		/// </summary>
		[Test]
		public void MatchSingleSwitch_01()
		{
			string commandline = HelperFunction.MakeCommandLine("-a");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
		}

		/// <summary>
		/// Match a command line having a single switch specified with the "--"
		/// delimiter.
		/// </summary>
		[Test]
		public void MatchSingleSwitch_02()
		{
			string commandline = HelperFunction.MakeCommandLine("--a");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
		}

		/// <summary>
		/// Match a command line having a single switch specified with the "/"
		/// delimiter.
		/// </summary>
		[Test]
		public void MatchSingleSwitch_03()
		{
			string commandline = HelperFunction.MakeCommandLine("/a");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by an equals sign.  The value is not
		/// enclosed by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_01()
		{
			string commandline = HelperFunction.MakeCommandLine("-a=b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by a colon.  The value is not enclosed
		/// by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_02()
		{
			string commandline = HelperFunction.MakeCommandLine("-a:b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by a space.  The value is not enclosed
		/// by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_03()
		{
			string commandline = HelperFunction.MakeCommandLine("-a b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by an equals sign.  The value is not
		/// enclosed by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_04()
		{
			string commandline = HelperFunction.MakeCommandLine("--a=b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a colon.  The value is not enclosed
		/// by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_05()
		{
			string commandline = HelperFunction.MakeCommandLine("--a:b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a space.  The value is not enclosed
		/// by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_06()
		{
			string commandline = HelperFunction.MakeCommandLine("--a b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by an equals sign.  The value is not
		/// enclosed by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_07()
		{
			string commandline = HelperFunction.MakeCommandLine("/a=b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by a colon.  The value is not enclosed
		/// by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_08()
		{
			string commandline = HelperFunction.MakeCommandLine("/a:b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by a space.  The value is not enclosed
		/// by any string delimiter.
		/// </summary>
		[Test]
		public void MatchSingleArgument_09()
		{
			string commandline = HelperFunction.MakeCommandLine("/a b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by an equals sign.  The value is enclosed
		/// by double quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_10()
		{
			string commandline = HelperFunction.MakeCommandLine("-a=\"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by a colon.  The value is enclosed
		/// by double quotes characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_11()
		{
			string commandline = HelperFunction.MakeCommandLine("-a:\"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by double quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_12()
		{
			string commandline = HelperFunction.MakeCommandLine("-a \"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by an equals sign.  The value is enclosed
		/// by double quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_13()
		{
			string commandline = HelperFunction.MakeCommandLine("--a=\"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a colon.  The value is enclosed
		/// by double quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_14()
		{
			string commandline = HelperFunction.MakeCommandLine("--a:\"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by double quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_15()
		{
			string commandline = HelperFunction.MakeCommandLine("--a \"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by an equals sign.  The value is enclosed
		/// by double quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_16()
		{
			string commandline = HelperFunction.MakeCommandLine("/a=\"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by a colon.  The value is enclosed
		/// by double quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_17()
		{
			string commandline = HelperFunction.MakeCommandLine("/a:\"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by double quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_18()
		{
			string commandline = HelperFunction.MakeCommandLine("/a \"b c\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"b c\"");
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by an equals sign.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_19()
		{
			string commandline = HelperFunction.MakeCommandLine("-a='b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by a colon.  The value is enclosed
		/// by single quotes characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_20()
		{
			string commandline = HelperFunction.MakeCommandLine("-a:'b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a single argument specified with the "-" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_21()
		{
			string commandline = HelperFunction.MakeCommandLine("-a 'b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by an equals sign.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_22()
		{
			string commandline = HelperFunction.MakeCommandLine("--a='b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a colon.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_23()
		{
			string commandline = HelperFunction.MakeCommandLine("--a:'b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_24()
		{
			string commandline = HelperFunction.MakeCommandLine("--a 'b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by an equals sign.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_25()
		{
			string commandline = HelperFunction.MakeCommandLine("/a='b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by a colon.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_26()
		{
			string commandline = HelperFunction.MakeCommandLine("/a:'b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a single argument specified with the "/" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchSingleArgument_27()
		{
			string commandline = HelperFunction.MakeCommandLine("/a 'b c'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'b c'");
		}

		/// <summary>
		/// Match a command line three switches (each specified using
		/// a different delimiter)
		/// </summary>
		[Test]
		public void MatchMultiSwitch_01()
		{
			string commandline = HelperFunction.MakeCommandLine("-a /b --c");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 3);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
			HelperFunction.ValidateParameter(match, 1, "b", String.Empty);
			HelperFunction.ValidateParameter(match, 2, "c", String.Empty);
		}

		/// <summary>
		/// Match a command line three switches (each specified using
		/// a different delimiter)
		/// </summary>
		[Test]
		public void MatchMultiSwitch_02()
		{
			string commandline = HelperFunction.MakeCommandLine("/a -b --c");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 3);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
			HelperFunction.ValidateParameter(match, 1, "b", String.Empty);
			HelperFunction.ValidateParameter(match, 2, "c", String.Empty);
		}

		/// <summary>
		/// Match a command line three switches (each specified using
		/// a different delimiter)
		/// </summary>
		[Test]
		public void MatchMultiSwitch_03()
		{
			string commandline = HelperFunction.MakeCommandLine("--a -b /c");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 3);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
			HelperFunction.ValidateParameter(match, 1, "b", String.Empty);
			HelperFunction.ValidateParameter(match, 2, "c", String.Empty);
		}

		/// <summary>
		/// Match three arguments each specified with a different starting
		/// sequence, separator, and value delimiter
		/// </summary>
		[Test]
		public void MatchMultiArgument_01()
		{
			string commandline = HelperFunction.MakeCommandLine("--a '1 2' /b=\"3 4\" -c:test");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 3);
			HelperFunction.ValidateParameter(match, 0, "a", "'1 2'");
			HelperFunction.ValidateParameter(match, 1, "b", "\"3 4\"");
			HelperFunction.ValidateParameter(match, 2, "c", "test");
		}

		/// <summary>
		/// Match three arguments each specified with a different starting
		/// sequence, separator, and value delimiter
		/// </summary>
		[Test]
		public void MatchMultiArgument_02()
		{
			string commandline = HelperFunction.MakeCommandLine("--a:'1 2' -b \"3 4\" /c=test");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 3);
			HelperFunction.ValidateParameter(match, 0, "a", "'1 2'");
			HelperFunction.ValidateParameter(match, 1, "b", "\"3 4\"");
			HelperFunction.ValidateParameter(match, 2, "c", "test");
		}

		/// <summary>
		/// Match three arguments each specified with a different starting
		/// sequence, separator, and value delimiter
		/// </summary>
		[Test]
		public void MatchMultiArgument_03()
		{
			string commandline = HelperFunction.MakeCommandLine("/a='1 2' -b:\"3 4\" --c test");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 3);
			HelperFunction.ValidateParameter(match, 0, "a", "'1 2'");
			HelperFunction.ValidateParameter(match, 1, "b", "\"3 4\"");
			HelperFunction.ValidateParameter(match, 2, "c", "test");
		}


		/// <summary>
		/// </summary>
		[Test]
		public void MatchNestedString_01()
		{
			string commandline = HelperFunction.MakeCommandLine("--a '\"b c\"'");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "'\"b c\"'");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchNestedString_02()
		{
			string commandline = HelperFunction.MakeCommandLine("--a \"'b c'\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "\"'b c'\"");
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchMixed_01()
		{
			string commandline = HelperFunction.MakeCommandLine("--a \"'1 2'\" /b");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 2);
			HelperFunction.ValidateParameter(match, 0, "a", "\"'1 2'\"");
			HelperFunction.ValidateParameter(match, 1, "b", String.Empty);
		}

		/// <summary>
		/// Match a single argument specified with the "--" prefix and the
		/// separated from its value by a space.  The value is enclosed
		/// by single quote characters.
		/// </summary>
		[Test]
		public void MatchMixed_02()
		{
			string commandline = HelperFunction.MakeCommandLine("--a /b:\"'1 2'\"");
			Match match = this.Regex.Match(commandline);
			HelperFunction.ValidateApplication(match);
			HelperFunction.ValidateArguments(match, 2);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
			HelperFunction.ValidateParameter(match, 1, "b", "\"'1 2'\"");
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ComplainOnStrange_01()
		{
			Assertion.Assert(!this.Regex.IsMatch(
				HelperFunction.MakeCommandLine("/a='1'extra")));
		}

# if FALSE
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ComplainOnStrange_02()
		{
			Assertion.Assert(!this.Regex.IsMatch(AppName + " /a=\"1\"extra"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ComplainOnStrange_03()
		{
			Assertion.Assert(!this.Regex.IsMatch(AppName + " /a='bad\""));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ComplainOnStrange_04()
		{
			Assertion.Assert(!this.Regex.IsMatch(AppName + " /a=\"bad'"));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ComplainOnStrange_05()
		{
			Assertion.Assert(!this.Regex.IsMatch(AppName + " /a=\"bad \"'"));

		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void ComplainOnStrange_06()
		{
			Assertion.Assert(!this.Regex.IsMatch(AppName + " /a='bad '\""));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Extraction_1()
		{
			MatchCollection matches = this.Regex.Matches("/a='1'");

			Assertion.Assert(matches.Count == 1);

			Assertion.Assert(matches[0].Groups["name"].Value == "a");
			Assertion.Assert(matches[0].Groups["value"].Value == "'1'");
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Extraction_2()
		{
			MatchCollection matches = this.Regex.Matches("/a=\"1\"");

			Assertion.Assert(matches.Count == 1);

			Assertion.Assert(matches[0].Groups["name"].Value == "a");
			Assertion.Assert(matches[0].Groups["value"].Value == "\"1\"");
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Extraction_3()
		{
			MatchCollection matches = this.Regex.Matches("/a='1 2'");

			Assertion.Assert(matches.Count == 1);

			Assertion.Assert(matches[0].Groups["name"].Value == "a");
			Assertion.Assert(matches[0].Groups["value"].Value == "'1 2'");
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Extraction_4()
		{
			string text = "/a=\"1 2\"";

			Assertion.Assert(this.Regex.IsMatch(text));

			MatchCollection matches = this.Regex.Matches(text);

			Assertion.Assert(matches.Count == 1);

			Assertion.Assert(matches[0].Groups["name"].Value == "a");
			Assertion.Assert(matches[0].Groups["value"].Value == "\"1 2\"");
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Extraction_5()
		{
			string text = "/a=\"1 2\" /b";

			Match match = this.Regex.Match(text);

			Assertion.Assert(match.Success);

			CaptureCollection names = match.Groups["name"].Captures;
			CaptureCollection values = match.Groups["value"].Captures;

			Assertion.Assert(names.Count == 2);
			Assertion.Assert(names.Count == values.Count);

			Assertion.Assert(names[0].Value == "a");
			Assertion.Assert(values[0].Value == "\"1 2\"");
			Assertion.Assert(names[1].Value == "b");
			Assertion.Assert(values[1].Value == String.Empty);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void Extraction_6()
		{
			string text = "/a=1 /b /c:'2' /d /e \"3\" /f \"has space\" /g 4  /h test -i=\"\" --j:''";

			Match match = this.Regex.Match(text);

			Assertion.Assert(match.Success);

			CaptureCollection names = match.Groups["name"].Captures;
			CaptureCollection values = match.Groups["value"].Captures;

			Assertion.Assert(names.Count == 10);
			Assertion.Assert(names.Count == values.Count);

			Assertion.Assert(names[0].Value == "a");
			Assertion.Assert(values[0].Value == "1");
			Assertion.Assert(names[1].Value == "b");
			Assertion.Assert(values[1].Value == String.Empty);
			Assertion.Assert(names[2].Value == "c");
			Assertion.Assert(values[2].Value == "'2'");
			Assertion.Assert(names[3].Value == "d");
			Assertion.Assert(values[3].Value == String.Empty);
			Assertion.Assert(names[4].Value == "e");
			Assertion.Assert(values[4].Value == "\"3\"");
			Assertion.Assert(names[5].Value == "f");
			Assertion.Assert(values[5].Value == "\"has space\"");
			Assertion.Assert(names[6].Value == "g");
			Assertion.Assert(values[6].Value == "4");
			Assertion.Assert(names[7].Value == "h");
			Assertion.Assert(values[7].Value == "test");
			Assertion.Assert(names[8].Value == "i");
			Assertion.Assert(values[8].Value == "\"\"");
			Assertion.Assert(names[9].Value == "j");
			Assertion.Assert(values[9].Value == "''");
		}
#endif
	}
}