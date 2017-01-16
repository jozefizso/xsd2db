using System;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Xsd2Db.CommandLineParser.Test
{
	/// <summary>
	/// Summary description for HelperFunction.
	/// </summary>
	public sealed class HelperFunction
	{
		/// <summary>
		/// The default application name to use for testing
		/// </summary>
		public const string ApplicationName = "test.exe";

		/// <summary>
		/// creates a command line with the given parameters
		/// </summary>
		/// <param name="parameters">the parameter string</param>
		/// <returns>a new command line string</returns>
		public static string MakeCommandLine(string parameters)
		{
			StringBuilder result = new StringBuilder();
			result.Append(ApplicationName);
			if (parameters != String.Empty)
			{
				result.AppendFormat(" {0}", parameters);
			}

			return result.ToString();
		}

		/// <summary>
		/// Checks if the expected name and value of the parameter is
		/// a match for the captured name and value at the given index.
		/// </summary>
		/// <param name="match">the regular expression match</param>
		/// <param name="index">the index of the captures name/value</param>
		/// <param name="expectedName">the expected name</param>
		/// <param name="expectedValue">the expected value</param>
		/// <exception cref="NUnit.Framework.AssertionException">
		///    Thrown if the actual name or value does not match the expected
		///    name or value, respectively.
		/// </exception>
		public static void ValidateParameter(
			Match match,
			int index,
			string expectedName,
			string expectedValue)
		{
			Assertion.Assert(
				"Match Failed",
				match.Success);

			string actualName = match.Groups[Pattern.GroupName.ParameterName].Captures[index].Value;
			string actualValue = match.Groups[Pattern.GroupName.ParameterValue].Captures[index].Value;

			Assertion.AssertEquals(
				String.Format(
					"Incorrect name!  Expected {0} but received {1}",
					expectedName, actualName),
				expectedName,
				actualName);

			Assertion.AssertEquals(
				String.Format(
					"Incorrect value!  Expected {0} but received {1}",
					expectedValue, actualValue),
				expectedValue,
				actualValue);
		}

		/// <summary>
		///   Checks that the command line was parsed correctly and that
		///   the application name matches the expected application name.
		/// </summary>
		/// <param name="match">the regular expression match</param>
		/// <exception cref="NUnit.Framework.AssertionException">
		///    Thrown if the actual application name does not match the
		///    expected application name, if an incorrect number of
		///    application names was found, or if the match did not
		///    succeed at all.
		/// </exception>
		/// <remarks>
		///   assumes the command line was created with the
		///   <see cref="HelperFunction.MakeCommandLine"/>
		///   function.
		/// </remarks>
		public static void ValidateApplication(Match match)
		{
			ValidateApplication(match, ApplicationName);
		}

		/// <summary>
		///   Checks that the command line was parsed correctly and that
		///   the application name matches the expected application name.
		/// </summary>
		/// <param name="match">the regular expression match</param>
		/// <param name="expectedName">the expected application name</param>
		/// <exception cref="NUnit.Framework.AssertionException">
		///    Thrown if the actual application name does not match the
		///    expected application name, if an incorrect number of
		///    application names was found, or if the match did not
		///    succeed at all.
		/// </exception>
		public static void ValidateApplication(Match match, string expectedName)
		{
			Assertion.Assert(
				"Match Failed",
				match.Success);

			Assertion.Assert(
				"Incorrect # of application matches found",
				match.Groups[Pattern.GroupName.ProgramName].Captures.Count == 1);

			Assertion.AssertEquals(
				"Application name is incorrect",
				ApplicationName,
				match.Groups[Pattern.GroupName.ProgramName].Captures[0].Value);
		}

		/// <summary>
		///   Checks if regular expression was successfully matched and if
		///   the correct number of parameter were found.
		/// </summary>
		/// <param name="match">
		///   the regular expression match
		/// </param>
		/// <param name="expectedParameterCount">
		///   the expected number of paramters
		/// </param>
		/// <exception cref="NUnit.Framework.AssertionException">
		///    Thrown if the actual parameter count does not match the expected
		///    paramter count, or if the match did not succeed at all.
		/// </exception>
		public static void ValidateArguments(
			Match match,
			int expectedParameterCount)
		{
			Assertion.Assert(
				"Match Failed",
				match.Success);

			Assertion.AssertEquals(
				"Wrong number of parameters found",
				expectedParameterCount,
				match.Groups[Pattern.GroupName.Parameter].Captures.Count);

			Assertion.AssertEquals(
				"Wrong number of names found",
				expectedParameterCount,
				match.Groups[Pattern.GroupName.ParameterName].Captures.Count);

			Assertion.AssertEquals(
				"Wrong number of values found",
				expectedParameterCount,
				match.Groups[Pattern.GroupName.ParameterValue].Captures.Count);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="regex"></param>
		/// <param name="text"></param>
		public static void ShouldMatch(Regex regex, string text)
		{
			Assertion.Assert(String.Format(
				"Should match {{{0}}}", text),
			                 regex.IsMatch(text));

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="regex"></param>
		/// <param name="text"></param>
		public static void ShouldNotMatch(Regex regex, string text)
		{
			Assertion.Assert(String.Format(
				"Should not match {{{0}}}", text),
			                 !regex.IsMatch(text));

		}
	}
}