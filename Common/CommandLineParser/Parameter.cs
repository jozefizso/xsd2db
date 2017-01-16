using System;
using System.Reflection;

namespace Xsd2Db.CommandLineParser
{
	/// <summary>
	/// Summary description for Parameter.
	/// </summary>
	public class Parameter : IComparable
	{
		#region -- Constructors -----------------------------------------------------

		/// <summary>
		/// Creates a new parameter record.
		/// </summary>
		/// <param name="name">
		///   The primary name by which this parameter is identified.
		/// </param>
		/// <param name="description">
		///   Descriptive text explaining the use/purpose of this parameter.
		/// </param>
		public Parameter(
			string name,
			string description)
			: this(name, String.Empty, description, IsCaseSensitiveDefault)
		{
			// nothing else to do
		}

		/// <summary>
		/// Creates a new paramter record.
		/// </summary>
		/// <param name="name">
		///   The primary name by which this parameter is identified.
		/// </param>
		/// <param name="alias">
		///   The alias by which this parameter is identified.
		/// </param>
		/// <param name="description">
		///   Descriptive text explaining the use/purpose of this parameter.
		/// </param>
		public Parameter(
			string name,
			string alias,
			string description)
			: this(name, alias, description, IsCaseSensitiveDefault)
		{
			// nothing else to do
		}

		/// <summary>
		/// Creates a new paramter record.
		/// </summary>
		/// <param name="name">
		///   The primary name by which this parameter is identified.
		/// </param>
		/// <param name="description">
		///   Descriptive text explaining the use/purpose of this parameter.
		/// </param>
		/// <param name="isCaseSensitive">
		///   True if the name and alias should be matches using case
		///   sensitivity.
		/// </param>
		public Parameter(
			string name,
			string description,
			bool isCaseSensitive)
			: this(name, String.Empty, description, isCaseSensitive)
		{
			// nothing else to do
		}

		/// <summary>
		///   Creates a new parameter record.
		/// </summary>
		/// <param name="name">
		///   The primary name by which this parameter is identified.
		/// </param>
		/// <param name="alias">
		///   The alias by which this parameter is identified.
		/// </param>
		/// <param name="description">
		///   Descriptive text explaining the use/purpose of this parameter.
		/// </param>
		/// <param name="isCaseSensitive">
		///   True if the name and alias should be matches using case
		///   sensitivity.
		/// </param>
		public Parameter(
			string name,
			string alias,
			string description,
			bool isCaseSensitive)
		{
			Helper.EnsureNotEmpty("name", name);
			Helper.EnsureNotEmpty("description", description);
			Helper.EnsureNotNull("alias", alias);

			this.Name = name;
			this.Alias = alias;
			this.Description = description;
			this.IsCaseSensitive = isCaseSensitive;
			this.valueIsPrefixedValue = false;
			this.ownerValue = null;
			this.memberValue = null;
		}

		#endregion Constructors -----------------------------------------------------

		#region -- Properties -------------------------------------------------------

		/// <summary>
		/// The primary name by which this parameter is identified.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// An optional alias by which this parameter is identified.
		/// </summary>
		public readonly string Alias;

		/// <summary>
		/// Descriptive text explaining the use/purpose of this parameter.
		/// </summary>
		public readonly string Description;

		/// <summary>
		/// True if the the name and alias of the parameter are to be
		/// matched using case sensitivity.
		/// </summary>
		public readonly bool IsCaseSensitive;

		/// <summary>
		/// Should parameters be case sensitive by default.
		/// </summary>
		public static bool IsCaseSensitiveDefault = true;

		/// <summary>
		/// True if this parameter has been bound to an owning instance
		/// and member.
		/// </summary>
		public bool IsBound
		{
			get
			{
				return (this.ownerValue != null)
					&& (this.memberValue != null);
			}
		}

		/// <summary>
		/// True if this parameter can be used to get the value of
		/// some bound member.
		/// </summary>
		public bool IsReadable
		{
			get
			{
				if (! this.IsBound) return true;

				PropertyInfo property = (this.Member as PropertyInfo);
				if (property != null)
				{
					return property.CanRead;
				}

				return true;
			}
		}

		/// <summary>
		/// True if this parameter can be used to set the value of
		/// some bound member.
		/// </summary>
		public bool IsWritable
		{
			get
			{
				if (! this.IsBound) return true;

				PropertyInfo property = (this.Member as PropertyInfo);
				if (property != null)
				{
					return property.CanWrite;
				}

				FieldInfo field = (this.Member as FieldInfo);
				if (field != null)
				{
					return !field.IsInitOnly;
				}

				throw new InvalidOperationException(
					"This parameter is bound to an unsupported member type");
			}
		}

		/// <summary>
		/// True if this paramter has an alias.
		/// </summary>
		public bool HasAlias
		{
			get { return (this.Alias != String.Empty); }
		}

		/// <summary>
		///   The object instance to which this parameter is bound.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   Thrown if the parameter is not bound to an owner and
		///   member.
		/// </exception>
		public object Owner
		{
			get
			{
				if (! this.IsBound)
				{
					throw new InvalidOperationException(
						"Parameter is not bound to an owner");
				}

				return this.ownerValue;
			}
		}

		/// <summary>
		///   The member field or property to which this parameter
		///   is bound.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   Thrown if the parameter is not bound to an owner and
		///   member.
		/// </exception>
		public MemberInfo Member
		{
			get
			{
				if (! this.IsBound)
				{
					throw new InvalidOperationException(
						"Parameter is not bound to a member");
				}

				return this.memberValue;
			}
		}

		/// <summary>
		///   True of the value of this parameter has been explicitly set.
		/// </summary>
		public bool IsExplicitlySet;

		/// <summary>
		/// 
		/// </summary>
		public bool HasType
		{
			get { return (this.typeValue != null); }
		}

		/// <summary>
		///   The type of value which this parameter accepts.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   If the parameter is bound, the type will automatically be set to
		///   that of the bound member.  It is an invalid operation to attempt
		///   to override that type assignment.
		/// </exception>
		public Type Type
		{
			get { return this.typeValue; }

			set
			{
				Helper.EnsureNotNull("value", value);

				if (this.IsBound)
				{
					throw new InvalidOperationException(
						"This parameter is bound to a member.  The type cannot be overridden.");
				}

				if (this.HasDefault && !value.IsAssignableFrom(this.Default.GetType()))
				{
					throw new ArgumentException(
						"The passed type does not match that of the default value");
				}

				this.typeValue = value;
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
			get
			{
				if (!this.HasDefault)
				{
					throw new InvalidOperationException(
						"This parameter does not have a default value");
				}

				return this.defaultValue;
			}

			set
			{
				Helper.EnsureNotNull("value", value);

				if (this.HasType && !this.Type.IsAssignableFrom(value.GetType()))
				{
					throw new ArgumentException(
						"The passed is not of a valid type");
				}

				this.defaultValue = value;
			}
		}

		/// <summary>
		/// True if there default value for this parameter.
		/// </summary>
		public bool HasDefault
		{
			get { return (this.defaultValue != null); }
		}

		/// <summary>
		/// True if the value of this parameter has been set.
		/// </summary>
		public bool HasValue
		{
			get { return (this.internalValue != null); }
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
		/// This property allows one to both set and retrieve the value
		/// of this parameter.  If the parameter is bound to an owning
		/// instance and member, the value of that member is directly
		/// used; otherwise, an internal vlaue cache is used.
		/// </summary>
		public object Value
		{
			//
			// If this parameter is not bound, then we should set the internal
			// value otherwise we should set the bound member's value.
			//
			set
			{
				// TODO: De we really want to do this?
				Helper.EnsureNotNull("value", value);

				if (! this.Type.IsAssignableFrom(value.GetType()))
				{
					throw new ArgumentException(
						"The passed is not of a valid type");
				}

				if (! this.IsBound)
				{
					this.internalValue = value;
				}
				else
				{
					switch (this.Member.MemberType)
					{
						case MemberTypes.Property:
							((PropertyInfo) this.Member).SetValue(this.Owner, value, null);
							break;
						case MemberTypes.Field:
							((FieldInfo) this.Member).SetValue(this.Owner, value);
							break;
						default:
							throw new InvalidOperationException(
								"This parameter is bound to an unsupported member type");
					}
				}

				this.IsExplicitlySet = true;
			}
			//
			// If this paramter is bound to an object and member, then we should
			// retrieve the stored value.  If this parameter is not bound, then
			// we should return the internal value if one is set, otherwise, an
			// exception should be thrown because the value has not been set.
			//
			get
			{
				if (this.IsBound)
				{
					switch (this.Member.MemberType)
					{
						case MemberTypes.Property:
							return ((PropertyInfo) this.Member).GetValue(this.Owner, null);
						case MemberTypes.Field:
							return ((FieldInfo) this.Member).GetValue(this.Owner);
						default:
							throw new InvalidOperationException(
								"This parameter is bound to an unsupported member type");
					}
				}
				else if (this.HasValue)
				{
					return this.internalValue;
				}

				throw new InvalidOperationException(
					"The value of this parameter has not been set");
			}
		}

		/// <summary>
		/// True if the resolved argument value of this parameter was prefixed
		/// by an @ character.
		/// </summary>
		public bool ValueIsPrefixed
		{
			get
			{
				if (!(this.IsBound || this.HasValue))
				{
					throw new InvalidOperationException(
						"This parameter does not yet have a value to prefix");
				}

				return this.valueIsPrefixedValue;
			}
			set
			{
				if (!(this.IsBound || this.HasValue))
				{
					throw new InvalidOperationException(
						"This parameter does not yet have a value to prefix");
				}

				this.valueIsPrefixedValue = value;
			}
		}

		#endregion Properties -------------------------------------------------------

		#region -- Methods ----------------------------------------------------------

		/// <summary>
		///   Bind this parameter to a member of an owning instance.
		///   The member can be either a property of a field.
		/// </summary>
		/// <param name="owner">
		///   The instance which owns the member
		/// </param>
		/// <param name="member">
		///   The member to which this parameter should be bound.
		/// </param>
		public void Bind(object owner, MemberInfo member)
		{
			Helper.EnsureNotNull("owner", owner);
			Helper.EnsureNotNull("member", member);

			switch (member.MemberType)
			{
				case MemberTypes.Property:
					this.Type = ((PropertyInfo) member).PropertyType;
					break;

				case MemberTypes.Field:
					this.Type = ((FieldInfo) member).FieldType;
					break;

				default:
					throw new ArgumentException(
						"The passed member type is not supported");
			}

			this.ownerValue = owner;
			this.memberValue = member;
		}

		/// <summary>
		///   Returns true if the given name is a match against this parameters
		///   name or its alias.  This comparison takes the case sensitivity of
		///   the parameter into account.
		/// </summary>
		/// <param name="name">the name to compare</param>
		/// <returns>
		///   true if the name is a match for this parameter's name or
		///   alias.  This comparison takes the case sensitivity of the
		///   parameter into account.
		/// </returns>
		public bool IsMatchFor(string name)
		{
			if ((this.Name == name) || (this.HasAlias && (this.Alias == name)))
			{
				return true;
			}

			if (!this.IsCaseSensitive)
			{
				name = name.ToLower();
				string myName = this.Name.ToLower();
				string myAlias = this.HasAlias
					? this.Alias.ToLower()
					: String.Empty;
				if ((myName == name) || (this.HasAlias && (myAlias == name)))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if this parameter is equavilent to the given parameter
		/// This means that the name or alias of one could be confused with the
		/// name or alias of the other.
		/// </summary>
		/// <param name="p">the other parameter</param>
		/// <returns>true if these represent the same parameter name</returns>
		public bool IsMatchFor(Parameter p)
		{
			return (this == p)
				|| this.IsMatchFor(p.Name)
				|| (p.HasAlias && this.IsMatchFor(p.Alias))
				|| p.IsMatchFor(this.Name)
				|| (this.HasAlias && p.IsMatchFor(this.Alias));
		}

		#endregion Methods ----------------------------------------------------------

		#region -- IComparable ------------------------------------------------------

		/// <summary>
		/// Compare this parameter with another parameter.
		/// </summary>
		/// <param name="otherObject">the object with which to compare</param>
		/// <returns></returns>
		int IComparable.CompareTo(object otherObject)
		{
			Parameter otherParam = otherObject as Parameter;
			if (otherParam == null)
			{
				throw new ArgumentException(
					"Object with which to compare has wrong type",
					"otherObject");
			}

			return this.Name.ToLower().CompareTo(otherParam.Name.ToLower());
		}

		#endregion IComparable ------------------------------------------------------

		#region -- Private Members --------------------------------------------------

		/// <summary>
		/// Private member variable that holds the object instance to
		/// which this parameter is bound.
		/// </summary>
		internal object ownerValue;

		/// <summary>
		/// Private member variable that holds the reflection information
		/// about the object member to which this parameter is bound.
		/// </summary>
		internal MemberInfo memberValue;

		/// <summary>
		/// Private member variable that holds the type of object which this
		/// parameter represents.
		/// </summary>
		internal Type typeValue;

		/// <summary>
		/// Private member variable that holds the value for this parameter
		/// if the parameter is not bound to some other member.
		/// </summary>
		internal object internalValue;

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
		/// Private member variable that trackes whether or not the resolved
		/// argument value for this parameter is prefixed by an @ sign.
		/// </summary>
		internal bool valueIsPrefixedValue;

		#endregion Private Members --------------------------------------------------
	}
}