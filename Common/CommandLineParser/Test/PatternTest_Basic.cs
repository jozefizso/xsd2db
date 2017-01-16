using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Xsd2Db.CommandLineParser.Test
{
	/// <summary>
	/// Performs basic testing on the sub-expressions of the command line pattern.
	/// </summary>
	public abstract class PatternTest_Basic
	{
		/// <summary>
		/// The application regex to use
		/// </summary>
		public readonly string ApplicationRegex;

		/// <summary>
		/// The parameter regex to use
		/// </summary>
		public readonly string ParameterRegex;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="applicationRegex">The application regex to use</param>
		/// <param name="parameterRegex">The parameter regex to use</param>
		public PatternTest_Basic(
			string applicationRegex,
			string parameterRegex)
		{
			this.ApplicationRegex = applicationRegex;
			this.ParameterRegex = parameterRegex;
		}

		/// <summary>
		/// Ensure that the application regular expression correctly identifies
		/// a program name.
		/// </summary>
		[Test]
		public void MatchProgramName()
		{
			string commandline = HelperFunction.MakeCommandLine(String.Empty);

			Regex programName = new Regex(this.ApplicationRegex);
			Match match = programName.Match(commandline);

			HelperFunction.ValidateApplication(match);
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "-" prefix.
		/// </summary>
		[Test]
		public void MatchSwitch_01()
		{
			string arguments = "-a";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "--" prefix.
		/// </summary>
		[Test]
		public void MatchSwitch_02()
		{
			string arguments = "--a";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "/" prefix.
		/// </summary>
		[Test]
		public void MatchSwitch_03()
		{
			string arguments = "/a";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", String.Empty);
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "-" prefix and a seperating space.
		/// </summary>
		[Test]
		public void MatchParameter_01()
		{
			string arguments = "-a b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "-" prefix and a seperating colon.
		/// </summary>
		[Test]
		public void MatchParameter_02()
		{
			string arguments = "-a:b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "-" prefix and a seperating equals sign.
		/// </summary>
		[Test]
		public void MatchParameter_03()
		{
			string arguments = "-a=b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "--" prefix and a separating space.
		/// </summary>
		[Test]
		public void MatchParameter_04()
		{
			string arguments = "--a b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "--" prefix and a separating colon.
		/// </summary>
		[Test]
		public void MatchParameter_05()
		{
			string arguments = "--a:b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "--" prefix and a separating equals sign.
		/// </summary>
		[Test]
		public void MatchParameter_06()
		{
			string arguments = "--a=b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "/" prefix and a seperating space.
		/// </summary>
		[Test]
		public void MatchParameter_07()
		{
			string arguments = "/a b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "/" prefix and a seperating colon.
		/// </summary>
		[Test]
		public void MatchParameter_08()
		{
			string arguments = "/a:b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}

		/// <summary>
		/// Ensures that the parameter regular expression correctly recognizes
		/// a switch using the "/" prefix and a seperating equals sign.
		/// </summary>
		[Test]
		public void MatchParameter_09()
		{
			string arguments = "/a=b";

			Regex argument = new Regex(this.ParameterRegex);
			Match match = argument.Match(arguments);
			HelperFunction.ValidateArguments(match, 1);
			HelperFunction.ValidateParameter(match, 0, "a", "b");
		}
	}

	/// <summary>
	/// Performs basic testing on the sub-expressions of the command line pattern
	/// using the basic regular expression for the parameter tests.
	/// 
	/// </summary>
	[TestFixture]
	public class PatternTest_Basic_UsingBasicPattern : PatternTest_Basic
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PatternTest_Basic_UsingBasicPattern()
			: base(Pattern.ApplicationPattern, Pattern.BasicParameterPattern)
		{
		}
	}

	/// <summary>
	/// Performs basic testing on the sub-expressions of the command line pattern
	/// using the advances regular expression for the parameter tests.
	/// </summary>
	[TestFixture]
	public class PatternTest_Basic_UsingAdvancedPattern : PatternTest_Basic
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PatternTest_Basic_UsingAdvancedPattern()
			: base(Pattern.ApplicationPattern, Pattern.AdvancedParameterPattern)
		{
		}


		/// <summary>
		/// The start of name pattern should match a single dash
		/// </summary>
		[Test]
		public void StartOfNamePatternTest()
		{
			Regex regex = new Regex("^" + Pattern.StartOfNamedParamPattern + "$");
			HelperFunction.ShouldMatch(regex, "-");
			HelperFunction.ShouldMatch(regex, "--");
			HelperFunction.ShouldMatch(regex, "/");
		}

		/// <summary>
		/// Test the NameValueSeparatorPattern
		/// </summary>
		[Test]
		public void NameValueSeparatorPatternTest()
		{
			Regex regex = new Regex("^" + Pattern.NameValueSeparatorPattern + "$");
			HelperFunction.ShouldMatch(regex, ":");
			HelperFunction.ShouldMatch(regex, " ");
			HelperFunction.ShouldMatch(regex, "   ");
			HelperFunction.ShouldMatch(regex, "=");
		}

		/// <summary>
		/// Test the IdentifirePattern
		/// </summary>
		public void IdentifierPatternTest()
		{
			Regex regex = new Regex("^" + Pattern.IdentifierPattern + "$");
			HelperFunction.ShouldMatch(regex, "roger123");
			HelperFunction.ShouldMatch(regex, "_hello");
			HelperFunction.ShouldMatch(regex, "funky_chicken");
			HelperFunction.ShouldMatch(regex, "_1_2_3");
			HelperFunction.ShouldNotMatch(regex, "Te.St.5_4");
			HelperFunction.ShouldNotMatch(regex, ".ab");
			HelperFunction.ShouldNotMatch(regex, ".ab");
			HelperFunction.ShouldNotMatch(regex, " ");
			HelperFunction.ShouldNotMatch(regex, "roger ");
			HelperFunction.ShouldNotMatch(regex, " roger");
			HelperFunction.ShouldNotMatch(regex, " roger ");
		}

		/// <summary>
		/// Test the NamePattern
		/// </summary>
		public void NamePatternTest()
		{
			Regex regex = new Regex("^" + Pattern.NamePattern + "$");
			HelperFunction.ShouldMatch(regex, "roger123");
			HelperFunction.ShouldMatch(regex, "_hello");
			HelperFunction.ShouldMatch(regex, "funky_chicken");
			HelperFunction.ShouldMatch(regex, "_1_2_3");
			HelperFunction.ShouldMatch(regex, "Te.St.5_4");
			HelperFunction.ShouldNotMatch(regex, ".ab");
			HelperFunction.ShouldNotMatch(regex, ".ab");
			HelperFunction.ShouldNotMatch(regex, " ");
			HelperFunction.ShouldNotMatch(regex, "roger ");
			HelperFunction.ShouldNotMatch(regex, " roger");
			HelperFunction.ShouldNotMatch(regex, " roger ");
		}

		/// <summary>
		/// Test the UnQuotedValuePattern
		/// </summary>
		[Test]
		public void UnQuotedValuePatternTest()
		{
			Regex regex = new Regex("^" + Pattern.UnQuotedValuePattern + "$");
			HelperFunction.ShouldMatch(regex, "roger123");
			HelperFunction.ShouldMatch(regex, "_hello");
			HelperFunction.ShouldMatch(regex, "funky_chicken");
			HelperFunction.ShouldMatch(regex, "_1_2_3");
			HelperFunction.ShouldMatch(regex, "Te.St.5_4");
			HelperFunction.ShouldMatch(regex, "\\");
			HelperFunction.ShouldMatch(regex, "C:\\Test");
			HelperFunction.ShouldMatch(regex, "C:\\Test\\");
			HelperFunction.ShouldMatch(regex, "\\\\Domain\\host");
			HelperFunction.ShouldMatch(regex, "Test\\ Directory");
			HelperFunction.ShouldMatch(regex, "@\\\\Domain\\host");
			HelperFunction.ShouldMatch(regex, "@Test\\ Directory");
			HelperFunction.ShouldNotMatch(regex, String.Empty);
			HelperFunction.ShouldNotMatch(regex, " ");
			HelperFunction.ShouldNotMatch(regex, "' '");
			HelperFunction.ShouldNotMatch(regex, "\" \"");
			HelperFunction.ShouldNotMatch(regex, "\\ .ab'");
			HelperFunction.ShouldNotMatch(regex, " ");
			HelperFunction.ShouldNotMatch(regex, "roger ");
			HelperFunction.ShouldNotMatch(regex, " roger");
			HelperFunction.ShouldNotMatch(regex, " roger ");
		}

		/// <summary>
		/// Test the SingleQuotedValuePattern
		/// </summary>
		[Test]
		public void SingleQuotedValuePatternTest()
		{
			Regex regex = new Regex("^" + Pattern.SingleQuotedValuePattern + "$");
			HelperFunction.ShouldMatch(regex, "'roger'");
			HelperFunction.ShouldMatch(regex, "'_hello'");
			HelperFunction.ShouldMatch(regex, "' test test'");
			HelperFunction.ShouldMatch(regex, "' foo\\ bar'");
			HelperFunction.ShouldMatch(regex, "' foo\" bar'");
			HelperFunction.ShouldMatch(regex, "@' foo\\ bar'");
			HelperFunction.ShouldMatch(regex, "@' foo\" bar'");
			HelperFunction.ShouldNotMatch(regex, "roger");
			HelperFunction.ShouldNotMatch(regex, "_hello");
			HelperFunction.ShouldNotMatch(regex, "' test test");
			HelperFunction.ShouldNotMatch(regex, "' foo\\ bar' '");
		}

		/// <summary>
		/// Test the DoubleQuotedValuePattern
		/// </summary>
		[Test]
		public void DoubleQuotedValuePatternTest()
		{
			Regex regex = new Regex("^" + Pattern.DoubleQuotedValuePattern + "$");
			HelperFunction.ShouldMatch(regex, "\"roger\"");
			HelperFunction.ShouldMatch(regex, "\"_hello\"");
			HelperFunction.ShouldMatch(regex, "\" test test\"");
			HelperFunction.ShouldMatch(regex, "\" foo\\ bar\"");
			HelperFunction.ShouldMatch(regex, "\" foo' bar\"");
			HelperFunction.ShouldMatch(regex, "@\" foo\\ bar\"");
			HelperFunction.ShouldMatch(regex, "@\" foo' bar\"");
			HelperFunction.ShouldNotMatch(regex, "roger");
			HelperFunction.ShouldNotMatch(regex, "_hello");
			HelperFunction.ShouldNotMatch(regex, "' test test");
			HelperFunction.ShouldNotMatch(regex, "' foo\\ bar' '");
		}

		/// <summary>
		/// Tests the ValuePattern
		/// </summary>
		[Test]
		public void ValuePatternTest()
		{
			Regex regex = new Regex("^" + Pattern.ValuePattern + "$");
			// Not quotes
			HelperFunction.ShouldMatch(regex, "roger123");
			HelperFunction.ShouldMatch(regex, "_hello");
			HelperFunction.ShouldMatch(regex, "funky_chicken");
			HelperFunction.ShouldMatch(regex, "_1_2_3");
			HelperFunction.ShouldMatch(regex, "Te.St.5_4");
			HelperFunction.ShouldMatch(regex, "\\");
			HelperFunction.ShouldMatch(regex, "C:\\Test");
			HelperFunction.ShouldMatch(regex, "C:\\Test\\");
			HelperFunction.ShouldMatch(regex, "\\\\Domain\\host");
			HelperFunction.ShouldMatch(regex, "Test\\ Directory");

			// Single quotes
			HelperFunction.ShouldMatch(regex, "'roger'");
			HelperFunction.ShouldMatch(regex, "'_hello'");
			HelperFunction.ShouldMatch(regex, "' test test'");
			HelperFunction.ShouldMatch(regex, "' foo\\ bar'");
			HelperFunction.ShouldMatch(regex, "' foo\" bar'");
			HelperFunction.ShouldMatch(regex, "@' foo\\ bar'");
			HelperFunction.ShouldMatch(regex, "@' foo\" bar'");

			// Double quotes
			HelperFunction.ShouldMatch(regex, "\"roger\"");
			HelperFunction.ShouldMatch(regex, "\"_hello\"");
			HelperFunction.ShouldMatch(regex, "\" test test\"");
			HelperFunction.ShouldMatch(regex, "\" foo\\ bar\"");
			HelperFunction.ShouldMatch(regex, "\" foo' bar\"");
			HelperFunction.ShouldMatch(regex, "@\" foo\\ bar\"");
			HelperFunction.ShouldMatch(regex, "@\" foo' bar\"");

			// Mismatches
			HelperFunction.ShouldNotMatch(regex, "\\ .ab'");
		}
	}

	/// <summary>
	/// Performs basic testing on the sub-expressions of the command line pattern
	/// using the basic regular expression for the parameter tests.
	/// 
	/// </summary>
	[TestFixture]
	public class PatternTest_Complex_UsingBasicPattern : PatternTest_Complex
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PatternTest_Complex_UsingBasicPattern()
			: base(Pattern.BasicRegex)
		{
		}
	}

	/// <summary>
	/// Performs basic testing on the sub-expressions of the command line pattern
	/// using the advances regular expression for the parameter tests.
	/// </summary>
	[TestFixture]
	public class PatternTest_Complex_UsingAdvancedPattern : PatternTest_Complex
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PatternTest_Complex_UsingAdvancedPattern()
			: base(Pattern.AdvancedRegex)
		{
		}
	}
}