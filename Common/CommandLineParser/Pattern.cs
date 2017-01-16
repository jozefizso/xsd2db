using System;
using System.Text.RegularExpressions;

namespace Xsd2Db.CommandLineParser
{
	/// <summary>
	/// Summary description for Parser.
	/// </summary>
	public sealed class Pattern
	{
		//#region -- Group Names (public) ---------------------------------------------

		/// <summary>
		/// The group names with which to extract pattern matches.
		/// </summary>
		public sealed class GroupName
		{
			/// <summary>
			/// The entire command line.
			/// </summary>
			public static readonly string CommandLine = "commandline";

			/// <summary>
			/// The entire command (this is the ProgramName plus any
			/// enclosing quotes)
			/// </summary>
			public static readonly string Command = "command";

			/// <summary>
			/// The program name.
			/// </summary>
			public static readonly string ProgramName = "program";

			/// <summary>
			/// A parameter (the entire name value pair)
			/// </summary>
			public static readonly string Parameter = "parameter";

			/// <summary>
			/// A parameter name
			/// </summary>
			public static readonly string ParameterName = "name";

			/// <summary>
			/// A parameter value
			/// </summary>
			public static readonly string ParameterValue = "value";
		}

		//#endregion Group Names ------------------------------------------------------

		//#region -- Implementation ---------------------------------------------------

		/// <summary>
		/// The pattern which starts a namee parameter: -, --, or /
		/// </summary>
		internal static readonly string StartOfNamedParamPattern = @"((\-\-)|(\-)|(\/))";

		/// <summary>
		/// The pattern which separates a parameter name from its
		/// value: =, :, or &lt;space&gt;
		/// </summary>
		internal static readonly string NameValueSeparatorPattern = @"([=:]|(\s+))";

		/// <summary>
		/// The pattern which matches an C style identifier
		/// </summary>
		internal static readonly string IdentifierPattern = @"(\w+)";

		/// <summary>
		/// The pattern which matches a parameter name: a(.b)* where a and b
		/// can be any valid C style identifier
		/// </summary>
		internal static readonly string NamePattern = String.Format(@"({0}(\.{0})*)", IdentifierPattern);

		/// <summary>
		/// The pattern which matches any unquoted parameter value.  This
		/// pattern allows for escaped space characters using the backslash.
		/// Pattern is a word character, period, backslash, or forward slash
		/// forward by any number of non-whitespace characters, escaped
		/// space characters that don't look they lead into another parameter,
		/// or backslash characters.
		/// </summary>
		internal static readonly string UnQuotedValuePattern = @"(@?[\w\\\.]([^\s\\]|(\\(\ [^-\/])?))*)";

		/// <summary>
		/// The pattern which matches a string enclosed in double quotes.
		/// May be preceded by an @ sign.  The application might want to
		/// use @ prefixed strings to denote escaped vs. literal string
		/// values.
		/// </summary>
		internal static readonly string DoubleQuotedValuePattern = "(@?\\\"[^\\\"]*\\\")";

		/// <summary>
		/// The pattern which matches a string enclosed in single quotes.
		/// May be preceded by an @ sign.  The application might want to
		/// use @ prefixed strings to denote escaped vs. literal string
		/// values.
		/// </summary>
		internal static readonly string SingleQuotedValuePattern = @"(@?'[^']*')";

		/// <summary>
		/// The value pattern to use when advanced matching is enabled.
		/// </summary>
		internal static readonly string ValuePattern = String.Format(
			"({0}|{1}|{2})",
			UnQuotedValuePattern,
			SingleQuotedValuePattern,
			DoubleQuotedValuePattern);

		/// <summary>
		/// The regular expression to match a parameter of the form
		/// -name=value where:
		/// <list type="*">
		/// <item><c>name</c> can be a(.b)* and a,b are C style identifiers</item>
		/// <item><c>-</c> can also be <c>/</c> or <c>--</c></item>
		/// <item><c>=</c> can also be <c>:</c> of <c>&lt;space&gt;</c></item>
		/// <item><c>value</c> is can containe escaped spaces or be
		/// either single or double quoted.  If quoted, value can be
		/// preceded by an <c>@</c></item>
		/// </list>
		/// </summary>
		internal static readonly string AdvancedParameterPattern = String.Format(
			"(?<{0}>{1}(?<{2}>{3})(({4}(?<{5}>{6}))|(?<{5}>.{{0}})))",
			new String[]
				{
					GroupName.Parameter,
					StartOfNamedParamPattern,
					GroupName.ParameterName,
					NamePattern,
					NameValueSeparatorPattern,
					GroupName.ParameterValue,
					ValuePattern
				});

		/// <summary>
		/// The regular expression to match a parameter of the form
		/// -name=value where:
		/// <list type="*">
		/// <item><c>name</c> can be a(.b)* and a,b are C style identifiers</item>
		/// <item><c>-</c> can also be <c>/</c> or <c>--</c></item>
		/// <item><c>=</c> can also be <c>:</c> of <c>&lt;space&gt;</c></item>
		/// <item><c>value</c> can also be single or double quoted</item>
		/// </list>
		/// </summary>
		internal static readonly string BasicParameterPattern = String.Format(
			"(?<{0}>(\\-\\-|\\-|\\/)(?<{1}>\\w+(\\.\\w+)*)(((=|:|\\s+)((?<{2}>\\\"[^\\\"]*\\\")|(?<{2}>'[^']*')|(?<{2}>[\\w\\\\\\.]\\S*)))|(?<{2}>.{{0}})))",
			GroupName.Parameter,
			GroupName.ParameterName,
			GroupName.ParameterValue);

		/// <summary>
		/// The regular expression to match an application name.
		/// </summary>
		internal static readonly string ApplicationPattern = String.Format(
			"(?<{0}>((?<{1}>\\S+)|(\\\"(?<{1}>[^\\\"]+)\\\")))",
			GroupName.Command,
			GroupName.ProgramName);

		/// <summary>
		/// The regular expression string to match a complete command line.
		/// </summary>
		internal static readonly string BasicCommandLinePattern = String.Format(
			"^(?<{0}>{1}(\\s+{2})*)$",
			GroupName.CommandLine,
			ApplicationPattern,
			BasicParameterPattern);

		/// <summary>
		/// The regular expression string to match a complete command line.
		/// </summary>
		internal static readonly string AdvancedCommandLinePattern = String.Format(
			"^(?<{0}>{1}(\\s+{2})*)$",
			GroupName.CommandLine,
			ApplicationPattern,
			AdvancedParameterPattern);

		/// <summary>
		/// The options with which to create the reqular expression parser.
		/// </summary>
		internal static readonly RegexOptions Options = RegexOptions.Compiled
			| RegexOptions.Singleline
			| RegexOptions.ExplicitCapture;

		/// <summary>
		/// The private member variable which stores the type of parser to use
		/// </summary>
#if ! USE_ADVANCED_PATTERNS
		internal static ParserType defaultParserTypeValue = ParserType.Basic;
#else
		internal static ParserType defaultParserTypeValue = ParserType.Advanced;
#endif
		//#endregion Implementation ---------------------------------------------------

		//#region -- Public Interface -------------------------------------------------

		/// <summary>
		/// The regular expression 
		/// </summary>
		public static readonly Regex BasicRegex
			= new Regex(BasicCommandLinePattern, Options);

		/// <summary>
		/// The regular expression 
		/// </summary>
		public static readonly Regex AdvancedRegex
			= new Regex(AdvancedCommandLinePattern, Options);

		/// <summary>
		/// The parser to use by default
		/// </summary>
		public enum ParserType
		{
			/// <summary>
			/// Use the basic parser.  It doesn't try to do as much.
			/// </summary>
			Basic,

			/// <summary>
			/// Use the advanced parser, it recognizes more command lines.
			/// </summary>
			Advanced
		}

		/// <summary>
		/// Get of set the default type of parser to use.
		/// </summary>
		public static ParserType DefaultParserType
		{
			get { return Pattern.defaultParserTypeValue; }
			set { Pattern.defaultParserTypeValue = value; }
		}

		/// <summary>
		/// Gets the parser corresponding to the given type.
		/// </summary>
		/// <param name="type">the type of parser</param>
		/// <returns>the parser corresponding to the given type.</returns>
		public static Regex GetParser(ParserType type)
		{
			switch (type)
			{
				case ParserType.Advanced:
					return AdvancedRegex;
				case ParserType.Basic:
					return BasicRegex;
			}

			throw new ArgumentException(String.Format(
				"The given parser type {{{0}}} is not supported", type),
			                            "type");
		}

		/// <summary>
		/// Get the parser of the current default type.
		/// </summary>
		public static Regex DefaultParser
		{
			get { return GetParser(DefaultParserType); }
		}

		//#endregion //Public Interface -------------------------------------------------
	}
}