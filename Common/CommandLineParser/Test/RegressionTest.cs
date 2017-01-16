using System;
using NUnit.Framework;

namespace Xsd2Db.CommandLineParser.Test
{
	/// <summary>
	/// Summary description for ParameterAttributeTest.
	/// </summary>
	[TestFixture]
	public class RegressionTest
	{
		/// <summary>
		/// A boolean parameter bound to a field and having a default value
		/// </summary>
		[Parameter("defaultBooleanField", "dbf", "A boolean parameter bound to a field and having a default value", Default=true, IsRequired=false)] public bool DefaultBooleanField;

		/// <summary>
		/// A boolean parameter bound to a property and having a default value
		/// </summary>
		[Parameter("defaultBooleanProperty", "dbp", "A boolean parameter bound to a property and having a default value", Default=true, IsRequired=false)] public bool DefaultBooleanProperty;

		/// <summary>
		/// A boolean parameter bound to a field
		/// </summary>
		[Parameter("booleanField", "bf", "A boolean parameter bound to a field", IsCaseSensitive=false)] public bool BooleanField;

		/// <summary>
		/// A integer parameter bound to a field
		/// </summary>
		[Parameter("integerField", "if", "An integer parameter bound to a field")] public int IntegerField;

		/// <summary>
		/// A DateTime parameter bound to a field
		/// </summary>
		[Parameter("dateTimeField", "df", "A DateTime parameter bound to a field")] public DateTime DateTimeField;

		/// <summary>
		/// A string parameter bound to a field
		/// </summary>
		[Parameter("stringField", "sf", "A string parameter bound to a field")] public string StringField;

		/// <summary>
		/// A boolean parameter bound to a property
		/// </summary>
		[Parameter("booleanProperty", "bp", "A boolean parameter bound to a property", IsCaseSensitive=false)]
		public bool BooleanProperty
		{
			get { return this.booleanValue; }
			set { this.booleanValue = value; }
		}

		/// <summary>
		/// The underlying value for <see cref="BooleanProperty"/>
		/// </summary>
		internal bool booleanValue;

		/// <summary>
		/// A integer parameter bound to a property
		/// </summary>
		[Parameter("integerProperty", "ip", "An integer parameter bound to a property")]
		public int IntegerProperty
		{
			get { return this.integerValue; }
			set { this.integerValue = value; }
		}

		/// <summary>
		/// The underlying value for <see cref="IntegerProperty"/>
		/// </summary>
		internal int integerValue;

		/// <summary>
		/// A DateTime parameter bound to a property
		/// </summary>
		[Parameter("dateTimeProperty", "dp", "A DateTime parameter bound to a property")]
		public DateTime DateTimeProperty
		{
			get { return this.dateTimeValue; }
			set { this.dateTimeValue = value; }
		}

		/// <summary>
		/// The underlying value for <see cref="DateTimeProperty"/>
		/// </summary>
		internal DateTime dateTimeValue;

		/// <summary>
		/// A string parameter bound to a property
		/// </summary>
		[Parameter("stringProperty", "sp", "A string parameter bound to a property")]
		public string StringProperty
		{
			get { return this.stringValue; }
			set { this.stringValue = value; }
		}

		/// <summary>
		/// The underlying value for <see cref="StringProperty"/>
		/// </summary>
		internal string stringValue;

		/// <summary>
		/// An enumeration to used when testing enumeration properties
		/// and fields.
		/// </summary>
		public enum Enumeration
		{
			/// <summary>
			/// The initial value.  All enum tests should validate that
			/// the field/property does not remain at this value
			/// </summary>
			Initial,

			/// <summary>
			/// Some test value
			/// </summary>
			Value1,

			/// <summary>
			/// Some test value
			/// </summary>
			Value2,

			/// <summary>
			/// Some test value
			/// </summary>
			Value3,

			/// <summary>
			/// Some test value
			/// </summary>
			Value4
		}

		/// <summary>
		/// An enumeration parameter bound to a field
		/// </summary>
		[Parameter("enumField", "ef", "An enumerated parameter bound to a field")] public Enumeration enumField;

		/// <summary>
		/// An enumeration parameter bound to a property
		/// </summary>
		[Parameter("enumProperty", "ep", "An enumerated parameter bound to a property")]
		public Enumeration enumProperty
		{
			get { return this.enumValue; }
			set { this.enumValue = value; }
		}

		/// <summary>
		/// The underlying value for <see cref="enumProperty"/>
		/// </summary>
		internal Enumeration enumValue;

		/// <summary>
		/// Initialize all fields and properties to a known state.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			this.DefaultBooleanField = false;
			this.BooleanField = false;
			this.IntegerField = 0;
			this.StringField = String.Empty;
			this.DateTimeField = DateTime.MinValue;
			this.enumField = Enumeration.Initial;

			this.DefaultBooleanProperty = this.DefaultBooleanField;
			this.BooleanProperty = this.BooleanField;
			this.IntegerProperty = this.IntegerField;
			this.DateTimeProperty = this.DateTimeField;
			this.StringProperty = this.StringField;
			this.enumProperty = enumField;
		}

		/// <summary>
		/// Ensure that the program name is parsed correctly.
		/// </summary>
		[Test]
		public void ProgramNameText()
		{
			string text = HelperFunction.MakeCommandLine(String.Empty);

			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			Assertion.Assert(
				"The program name is incorrect",
				c.ProgramName == HelperFunction.ApplicationName);
		}

		/// <summary>
		/// Ensre that that the boolean field is set correctly
		/// </summary>
		[Test]
		public void BooleanFieldTest_01()
		{
			string text = HelperFunction.MakeCommandLine("-booleanField=true");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.BooleanField);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void BooleanFieldTest_02()
		{
			string text = HelperFunction.MakeCommandLine("-bF=true");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.BooleanField);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void BooleanFieldTest_03()
		{
			string text = HelperFunction.MakeCommandLine("-booleanField='True'");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.BooleanField);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void BooleanFieldTest_04()
		{
			string text = HelperFunction.MakeCommandLine("--Bf:'Yes'");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.BooleanField);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void BooleanPropertyTest_01()
		{
			string text = HelperFunction.MakeCommandLine("-booleanProperty=true");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.BooleanProperty);
		}

		/// <summary>
		/// Ensure that the boolean property is set using its alias if
		/// 'true' is the argument value is supplied on the command line.
		/// The alias uses different case, and the parameter is not case
		/// sensitive.
		/// </summary>
		[Test]
		public void BooleanPropertyTest_02()
		{
			string text = HelperFunction.MakeCommandLine("-bP=true");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.BooleanProperty);
		}

		/// <summary>
		/// Ensure that the boolean property is set using its full name if
		/// 'True' is the argument value is supplied on the command line.
		/// </summary>
		[Test]
		public void BooleanPropertyTest_03()
		{
			string text = HelperFunction.MakeCommandLine("-booleanProperty='True'");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.BooleanProperty);
		}

		/// <summary>
		/// Ensure that the boolean property is set using its alias if 'Yes'
		/// is the argument value is supplied on the command line.
		/// </summary>
		[Test]
		public void BooleanPropertyTest_04()
		{
			string text = HelperFunction.MakeCommandLine("--Bp:'Yes'");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.BooleanProperty);
		}


		/// <summary>
		/// Ensure that the default value of the boolean field is set if
		/// the matching argument is not supplied on the command line.
		/// </summary>
		[Test]
		public void DefaultBooleanFieldTest()
		{
			string text = HelperFunction.MakeCommandLine("--someName:'SomeValue'");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.DefaultBooleanField);
		}

		/// <summary>
		/// Ensure that the default value of the boolean property is set if
		/// the matching argument is not supplied on the command line.
		/// </summary>
		[Test]
		public void DefaultBooleanPropertyTest()
		{
			string text = HelperFunction.MakeCommandLine("--someName:'SomeValue'");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.Assert(this.DefaultBooleanProperty);
		}

		/// <summary>
		/// Ensure that the enumeration field gets correctly set using
		/// its full name.
		/// </summary>
		[Test]
		public void EnumFieldTest_01()
		{
			string text = HelperFunction.MakeCommandLine("--enumField:'value1'");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.AssertEquals(
				"Wrong enumeration value observed",
				Enumeration.Value1,
				this.enumField);
		}

		/// <summary>
		/// Ensure that the enumeration field gets correctly set using
		/// its alias.
		/// </summary>
		[Test]
		public void EnumFieldTest_02()
		{
			string text = HelperFunction.MakeCommandLine("--ef:Value2");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.AssertEquals(
				"Wrong enumeration value observed",
				Enumeration.Value2,
				this.enumField);
		}

		/// <summary>
		/// Ensure that the enumeration property gets correctly set using
		/// its full name even if the case used to specify the enumeration
		/// value is different from the enumeration value (i.e., matching
		/// should be not be case sensitive)
		/// </summary>
		[Test]
		public void EnumPropertyTest_01()
		{
			string text = HelperFunction.MakeCommandLine("--enumProperty:\"vaLUE3\"");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.AssertEquals(
				"Wrong enumeration value observed",
				Enumeration.Value3,
				this.enumProperty);
		}

		/// <summary>
		/// Ensure that the enumeration property gets correctly set using
		/// its alias even if the case used to specify the enumeration
		/// value is different from the enumeration value (i.e., matching
		/// should be not be case sensitive)
		/// </summary>
		[Test]
		public void EnumPropertyTest_02()
		{
			string text = HelperFunction.MakeCommandLine("--ep:\"vALUe4\"");
			CommandLine c = new CommandLine(text);

			Assertion.Assert(
				"The command line text failed to parse",
				c.TextIsValid);

			c.Bind(this);
			c.Resolve();
			Assertion.AssertEquals(
				"Wrong enumeration value observed",
				Enumeration.Value4,
				this.enumProperty);
		}
	}
}