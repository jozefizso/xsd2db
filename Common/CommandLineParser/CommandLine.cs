using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xsd2Db.CommandLineParser
{
	/// <summary>
	/// Summary description for CommandLine.
	/// </summary>
	public class CommandLine
	{
		#region -- Constructors -----------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		public CommandLine()
			: this(Environment.CommandLine)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public CommandLine(string text)
		{
			//
			// Internal values for readonly properties
			//

			this.argumentCollection = new ArgumentCollection();
			this.parameterCollection = new ParameterCollection();
			this.programNameValue = String.Empty;

			//
			// Read/Write Properties
			//

			this.ParserType = Pattern.DefaultParserType;
			this.FormatProvider = CultureInfo.CurrentCulture;
			this.AutoValidate = false;
			this.AllowUnrecognized = true;
			this.Text = text;
		}

		#endregion Constructors -----------------------------------------------------

		#region -- Properties--------------------------------------------------------

		/// <summary>
		/// True if the parser should allow arguments for which no corresponding
		/// parameter is found to be accepted.  These arguments can be found by
		/// looking through the <see cref="Arguments"/> collection.
		/// </summary>
		public bool AllowUnrecognized
		{
			get { return this.allowUnrecognizedValue; }
			set { this.allowUnrecognizedValue = value; }
		}

		/// <summary>
		/// True if the parser should automatically valicate the parameters
		/// after they have been resolved.
		/// </summary>
		public bool AutoValidate
		{
			get { return this.autoValidateValue; }
			set { this.autoValidateValue = value; }
		}

		/// <summary>
		/// The collection of arguments which have been parsed by this
		/// parser.
		/// </summary>
		public ArgumentCollection Arguments
		{
			get { return this.argumentCollection; }
		}


		/// <summary>
		/// The collection of parameters which this parser can recognize.
		/// </summary>
		public ParameterCollection Parameters
		{
			get { return this.parameterCollection; }
		}

		/// <summary>
		/// The name of the program that is specified on this command line.
		/// </summary>
		public string ProgramName
		{
			get { return this.programNameValue; }
		}

		/// <summary>
		/// The format provider the parser should use for converting the
		/// textual arguments into typed parameters.
		/// </summary>
		public IFormatProvider FormatProvider
		{
			get { return this.formatProviderValue; }
			set
			{
				Helper.EnsureNotNull("value", value);
				this.formatProviderValue = value;
			}
		}

		/// <summary>
		/// Allows you to get or set the type of parser to use
		/// </summary>
		public Pattern.ParserType ParserType
		{
			get { return this.parserTypeValue; }
			set { this.parserTypeValue = value; }
		}

		/// <summary>
		/// Get and set the command line text.  After setting the text, you
		/// should examine the TextIsValid property to see if the text was
		/// parsed properly.
		/// </summary>
		public string Text
		{
			get { return this.textValue; }
			set
			{
				Helper.EnsureNotNull("value", value);

				//
				// Update the text values
				//

				this.textValue = value.Trim();
				this.textIsValidValue = false;
				this.programNameValue = String.Empty;
				this.Arguments.Clear();

				//
				// Parse the command line text
				//

				Match match = Pattern.GetParser(this.ParserType).Match(this.textValue);

				this.textIsValidValue = match.Success;

				if (this.TextIsValid)
				{
					CaptureCollection programs = match.Groups[Pattern.GroupName.ProgramName].Captures;
					CaptureCollection names = match.Groups[Pattern.GroupName.ParameterName].Captures;
					CaptureCollection values = match.Groups[Pattern.GroupName.ParameterValue].Captures;

					Debug.Assert(programs.Count == 1);
					Debug.Assert(names.Count == values.Count);

					this.programNameValue = TrimQuotes(programs[0].Value);
					for (int i = 0; i < names.Count; ++i)
					{
						this.Arguments.Add(names[i].Value, values[i].Value);
					}
				}
			}
		}

		/// <summary>
		/// True if the current command line text has been successfully parsed
		/// </summary>
		public bool TextIsValid
		{
			get { return this.textIsValidValue; }
		}

		#endregion Properties--------------------------------------------------------

		#region -- Methods ----------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="application"></param>
		public void Bind(object application)
		{
			Bind(application, false);
		}

		/// <summary>
		/// Populates the collection of recognized parameters based on the
		/// attributes that have been applied to the given objects members.
		/// </summary>
		/// <param name="application">
		///   the application object whose members may be tagged with
		///   <see cref="ParameterAttribute">ParameterAttributes</see>
		/// </param>
		/// <param name="inherit">
		///   true if the parser should also consider attributes which are
		///   inhereted from base classes of the application.
		/// </param>
		public void Bind(object application, bool inherit)
		{
			Type type = application.GetType();
			MemberInfo[] members = type.GetMembers();

			foreach (MemberInfo member in members)
			{
				foreach (Attribute customAttribute in member.GetCustomAttributes(inherit))
				{
					ParameterAttribute attribute = customAttribute as ParameterAttribute;

					if (attribute != null)
					{
						Parameter parameter = new Parameter(
							attribute.Name,
							attribute.HasAlias ? attribute.Alias : String.Empty,
							attribute.Description,
							attribute.IsCaseSensitive);

						parameter.Bind(application, member);

						if (attribute.HasDefault)
						{
							parameter.Default = attribute.Default;
						}

						parameter.IsRequired = attribute.IsRequired;

						this.Parameters.Add(parameter);
					}
				}
			}
		}

		/// <summary>
		/// Goes through the arguments and matches them to the corresponding
		/// parameters.
		/// </summary>
		public void Resolve()
		{
			foreach (Argument argument in this.Arguments)
			{
				Parameter parameter = this.Parameters[argument.Name];

				if (parameter != null)
				{
					Assign(parameter, argument.Value);
				}
				else if (! this.AllowUnrecognized)
				{
					throw new ArgumentException(String.Format(
						"Unrecognized argument: {0} = {1}",
						argument.Name,
						argument.Value));
				}
			}

			foreach (Parameter parameter in this.Parameters)
			{
				if (!parameter.IsExplicitlySet && parameter.HasDefault)
				{
					parameter.Value = parameter.Default;

					//
					// Reset the flag to say that this parameter was
					// not explicitly set by the user.
					//

					parameter.IsExplicitlySet = false;
				}
			}

			if (this.AutoValidate)
			{
				Validate();
			}
		}

		/// <summary>
		/// Goes through the parameters and makes sure that all required
		/// parameters have been explicitly set in the command line.
		/// </summary>
		public void Validate()
		{
			foreach (Parameter param in this.Parameters)
			{
				if (!param.IsExplicitlySet && param.IsRequired)
				{
					throw new ArgumentException(
						"Required parameter is not set",
						param.Name);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void WriteHelp(TextWriter writer)
		{
			this.Parameters.Sort();
			foreach (Parameter parameter in this.Parameters)
			{
				writer.Write("  -{0}", parameter.Name);
				if (parameter.HasAlias)
				{
					writer.Write(" (-{0})", parameter.Alias);
				}
				writer.WriteLine();
				writer.WriteLine("\t{0}", parameter.Description.Replace("\n", writer.NewLine + "\t"));
			}
		}

		#endregion Methods ----------------------------------------------------------

		#region -- Private Members --------------------------------------------------

		/// <summary>
		/// Converts the given text into the appropriate type for the given
		/// parameter and sets the parameter's value.
		/// </summary>
		/// <param name="param">the parameter to be set</param>
		/// <param name="text">the textual value to be parsed</param>
		internal void Assign(Parameter param, string text)
		{
			bool valueIsPrefixed = false;
			bool succeeded = true;

			try
			{
				//
				// If the argument is just a flag (i.e., no value is
				// specified.  And this is a boolean paramter, then
				// we take that to mean that the user wishes to reverse
				// the default polarity of that switch.
				//

				if (text.Equals(String.Empty)
					&& param.Type.Equals(typeof (bool))
					&& param.HasDefault)
				{
					param.Value = !((bool) param.Default);
					return;
				}

				//
				// Check for a prefix
				//

				if (text.StartsWith("@"))
				{
					valueIsPrefixed = true;
					text = text.Substring(1);
				}

				text = TrimQuotes(text);

				//
				// Support a bunch of alternative values for boolean arguments.
				//

				if (param.Type.Equals(typeof (bool)))
				{
					switch (text.ToLower())
					{
						case "1":
						case "t":
						case "+":
						case "y":
						case "yes":
							param.Value = true;
							return;
						case "0":
						case "f":
						case "-":
						case "n":
						case "no":
							param.Value = false;
							return;
					}
				}

				//
				// Otherwise, try to convert the text into the appropriate type.
				//

				if (param.Type.IsEnum)
				{
					param.Value = Enum.Parse(param.Type, text, true);
				}
				else
				{
					param.Value = ((IConvertible) text).ToType(param.Type, this.FormatProvider);
				}
			}
			catch (Exception)
			{
				succeeded = false;
				throw;
			}
			finally
			{
				if (succeeded)
				{
					param.ValueIsPrefixed = valueIsPrefixed;
				}
			}
		}

		/// <summary>
		/// If the argument is framed by single or double quotes, trim them
		/// out to just get the string's contents.
		/// </summary>
		/// <param name="text">the text to be trimmed</param>
		/// <returns>
		/// The input string without enclosing quotation marks
		/// </returns>
		internal string TrimQuotes(string text)
		{
			string SingleQuote = "'";
			string DoubleQuote = "\"";
			if ((text.StartsWith(SingleQuote) && text.EndsWith(SingleQuote))
				|| (text.StartsWith(DoubleQuote) && text.EndsWith(DoubleQuote)))
			{
				return text.Substring(1, text.Length - 2);
			}

			return text.Replace("\\ ", " ");
		}

		/// <summary>
		/// The private member variable that tracks whether or not arguments
		/// for which no corresponding parameter specification is found should
		/// be allowed.  The state of this parameter is managed by the
		/// AllowUnrecognized property.
		/// </summary>
		internal bool allowUnrecognizedValue;

		/// <summary>
		/// The private member variable that tracks whether or not the
		/// parameters should be automatically valicated after they are
		/// resolved. The state of this parameter is managed by the
		/// AutoValidate property.
		/// </summary>
		internal bool autoValidateValue;

		/// <summary>
		/// The private member variable that stores the collection of
		/// arguments which have been parsed by this parsed.  This
		/// variable is accessible through the Arguments property and
		/// its contents are manages by the set method for the Text
		/// property.
		/// </summary>
		internal readonly ArgumentCollection argumentCollection;

		/// <summary>
		/// The private member variable that stores the collection of
		/// parameter specifications which the CommandLine can recognize.
		/// This variable is accessible through the Parameters property.
		/// </summary>
		internal readonly ParameterCollection parameterCollection;

		/// <summary>
		/// The private member variable that stores format provider the
		/// parser should use for converting the textual arguments into
		/// typed parameters.  The value of this variable is managed by
		/// the FormatProvider property.
		/// </summary>
		internal IFormatProvider formatProviderValue;

		/// <summary>
		///   The private member variable that stores the program name that
		///   is specified on this command line.  The state of this variable
		///   is managed by the set method for the Text property.
		/// </summary>
		internal string programNameValue;

		/// <summary>
		///   The private member variable that tracks the current textual
		///   value of this command line.  The state of this variable is
		///   managed by the set method for the Text property.
		/// </summary>
		internal string textValue;

		/// <summary>
		///   The private member variable that tracks whether or not the
		///   current textual value of this command line parses correctly.
		///   The state of this variable is managed by the set method for
		///   the Text property.
		/// </summary>
		internal bool textIsValidValue;

		/// <summary>
		///   The private member variable that stores the type of parser to
		///   use.
		/// </summary>
		internal Pattern.ParserType parserTypeValue;

		#endregion Private Members --------------------------------------------------
	}
}