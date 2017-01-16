using System;

namespace Xsd2Db.CommandLineParser
{
	/// <summary>
	/// Summary description for Parameter.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Property | AttributeTargets.Field,
		Inherited = false,
		AllowMultiple = false)]
	public class ParameterAttribute : Attribute
	{
		/// <summary>
		/// Internal constructor called by other constructors.  Creates a new
		/// Parameter specification attribute.
		/// </summary>
		/// <param name="name">
		///   The full name for this parameter.  May not be empty or null.
		/// </param>
		/// <param name="alias">
		///   An equivalent abbreviated name for this parameter.  May not be
		///   null.  Use String.Empty to indicate that there is not alias for
		///   this parameter.
		/// </param>
		/// <param name="description">
		///   Explanatory text about this parameter.  May not be empty or null.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If any of the string parameters are null
		/// </exception>
		/// <exception cref="ArgumentException">
		///   If a required string parameter is the empty string
		/// </exception>
		public ParameterAttribute(
			string name,
			string alias,
			string description)
		{
			Helper.EnsureNotEmpty("name", name);
			Helper.EnsureNotEmpty("description", description);
			Helper.EnsureNotNull("alias", alias);

			this.Name = name;
			this.Description = description;
			this.Alias = alias;

			this.defaultValue = null;
			this.isRequiredValue = false;
			this.caseSensitivityIsSet = false;
		}

		/// <summary>
		///   Creates a parameter specification attribute for an optional
		///   parameter
		/// </summary>
		/// <param name="name">
		///   The full name for this parameter.  May not be empty or null.
		/// </param>
		/// <param name="description">
		///   Explanatory text about this parameter.  May not be empty or null.
		/// </param>
		public ParameterAttribute(
			string name,
			string description)
			: this(name, String.Empty, description)
		{
		}

		/// <summary>
		///   The long form of the parameter.
		///   Will be a non-empty string.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// An abbreviated form of the parameter.    May be the empty string.
		/// The empty string signigies that no abbreviated form exists.
		/// </summary>
		public readonly string Alias;

		/// <summary>
		/// The text to use when constructing the usage instructions for this
		/// parameter.  This string will be non-empty.
		/// </summary>
		public readonly string Description;

		/// <summary>
		/// True if there default value for this parameter.
		/// </summary>
		public bool HasDefault
		{
			get { return (this.defaultValue != null); }
		}

		/// <summary>
		/// True if there is a valid short form for this value
		/// </summary>
		public bool HasAlias
		{
			get { return (this.Alias != String.Empty); }
		}

		/// <summary>
		/// True if there this parameter has been flagges as a required
		/// value by the user.
		/// </summary>
		public bool IsRequired
		{
			set { this.isRequiredValue = value; }
			get { return this.isRequiredValue; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsCaseSensitive
		{
			set
			{
				this.isCaseSensitiveValue = value;
				this.caseSensitivityIsSet = true;
			}
			get
			{
				if (this.caseSensitivityIsSet)
					return this.isCaseSensitiveValue;
				return Parameter.IsCaseSensitiveDefault;
			}
		}

		/// <summary>
		/// The default value of this parameter.  This value should be used
		/// if the parameter is not specified by the user.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   Thrown if the parmeter does not have a default value.
		/// </exception>
		public object Default
		{
			set
			{
				Helper.EnsureNotNull("value", value);
				this.defaultValue = value;
			}

			get
			{
				if (!this.HasDefault)
				{
					throw new InvalidOperationException(
						"This parameter does not have a default value");
				}

				return (this.defaultValue);
			}
		}

		/// <summary>
		/// Private member variable that holds the default value for this
		/// parameter or null (if no default value is set).
		/// </summary>
		internal object defaultValue;

		/// <summary>
		/// Private member variable that denotes whether or not this parameter
		/// has been flagged as a required value by the user.
		/// </summary>
		internal bool isRequiredValue;

		/// <summary>
		/// Private member variable that denotes whether or not this parameter's
		/// name should be considered case sensitive.
		/// </summary>
		internal bool isCaseSensitiveValue;

		/// <summary>
		/// Private member variable that denotes whether or not this parameter
		/// </summary>
		internal bool caseSensitivityIsSet;
	}
}