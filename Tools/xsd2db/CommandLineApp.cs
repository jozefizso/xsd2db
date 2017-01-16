using System;

namespace xsd2db
{
	/// <summary>
	/// This class is intended to be used as a base class for other command
	/// line applications.
	/// </summary>
	public class CommandLineApp
	{
        /// <summary>
        /// Constructs a new command line application base class.
        /// </summary>
        /// <param name="args">the command line arguments</param>
		public CommandLineApp( string[] args)
		{
			m_Arguments = new Arguments( args );
		}

		
        /// <summary>
        /// A parsed map of the arguments passed on the command line.
        /// </summary>
        protected Arguments m_Arguments;

		/// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// string.
		/// </summary>
		/// <param name="name">the name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a string</returns>
		protected String ParamAsString(
            string name,
            bool   isRequired,
            string defValue )
		{
			string text = m_Arguments[name];
			if (text == null)
			{
				if (isRequired)
				{
					throw new ArgumentException(
						"Required parameter '" + name + "' not specified",
						name);
				}

				text = defValue;
			}

			return text;
		}

		
        /// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// string.
		/// </summary>
		/// <param name="longName">the full name of this parameter</param>
		/// <param name="shortName">the abbreviated name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a string</returns>
		protected String ParamAsString(
			string longName,
			string shortName,
			bool   isRequired,
			string defValue )
		{
			string text = ParamAsString( shortName, false, null );
			if (text == null)
			{
				text = ParamAsString( longName, isRequired, defValue );
			}
            return text;
		}

		/// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// boolean.
		/// </summary>
		/// <param name="name">the name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a boolean</returns>
		protected Boolean ParamAsBoolean(
			string name,
			bool   isRequired,
			bool   defValue )
		{
			try
			{
				return Boolean.Parse( ParamAsString(
					name,
					isRequired,
					defValue ? Boolean.TrueString : Boolean.FalseString));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", name, thrown);
			}
		}

		/// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// boolean.
		/// </summary>
		/// <param name="longName">the full name of this parameter</param>
		/// <param name="shortName">the abbreviated name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a boolean</returns>
		protected Boolean ParamAsBoolean(
			string longName,
			string shortName,
			bool   isRequired,
			bool   defValue )
		{
			try
			{
				return Boolean.Parse( ParamAsString(
					longName,
					shortName,
					isRequired,
					defValue ? Boolean.TrueString : Boolean.FalseString));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", longName, thrown);
			}
		}

		/// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// 32 bit signed integer.
		/// </summary>
		/// <param name="name">the name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a 32 bit signed integer</returns>
		protected Int32 ParamAsInt32(
			string name,
			bool   isRequired,
			int    defValue )
		{
			try
			{
				return Int32.Parse( ParamAsString(
					name,
					isRequired,
					String.Format("{0}", defValue)));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", name, thrown);
			}
		}

		/// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// 32 bit signed integer.
		/// </summary>
		/// <param name="longName">the full name of this parameter</param>
		/// <param name="shortName">the abbreviated name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a 32 bit signed integer</returns>
		protected Int32 ParamAsInt32(
			string longName,
			string shortName,
			bool   isRequired,
			int    defValue )
		{
			try
			{
				return Int32.Parse( ParamAsString(
					longName,
					shortName,
					isRequired,
					String.Format("{0}", defValue)));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", longName, thrown);
			}
		}


        /// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// 32 bit unsigned integer.
		/// </summary>
		/// <param name="name">the name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a 32 bit unsigned integer</returns>
		protected UInt32 ParamAsUInt32(
			string name,
			bool   isRequired,
			int    defValue )
		{
			try
			{
				return UInt32.Parse( ParamAsString(
					name,
					isRequired,
					String.Format("{0}", defValue)));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", name, thrown);
			}
		}


        /// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// 32 bit unsigned integer.
		/// </summary>
		/// <param name="longName">the full name of this parameter</param>
		/// <param name="shortName">the abbreviated name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a 32 bit unsigned integer</returns>
		protected UInt32 ParamAsUInt32(
			string longName,
			string shortName,
			bool   isRequired,
			int    defValue )
		{
			try
			{
				return UInt32.Parse( ParamAsString(
					longName,
					shortName,
					isRequired,
					String.Format("{0}", defValue)));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", longName, thrown);
			}
		}
		
        /// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// 16 bit signed integer.
		/// </summary>
		/// <param name="name">the name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a 16 bit signed integer</returns>
		protected Int16 ParamAsInt16(
			string name,
			bool   isRequired,
			int    defValue )
		{
			try
			{
				return Int16.Parse( ParamAsString(
					name,
					isRequired,
					String.Format("{0}", defValue)));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", name, thrown);
			}
		}

		/// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// 16 bit signed integer.
		/// </summary>
		/// <param name="longName">the full name of this parameter</param>
		/// <param name="shortName">the abbreviated name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a 16 bit signed integer</returns>
		protected Int16 ParamAsInt16(
			string longName,
			string shortName,
			bool   isRequired,
			int    defValue )
		{
			try
			{
				return Int16.Parse( ParamAsString(
					longName,
					shortName,
					isRequired,
					String.Format("{0}", defValue)));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", longName, thrown);
			}
		}

		
        /// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// 16 bit unsigned integer.
		/// </summary>
		/// <param name="name">the name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a 16 bit unsigned integer</returns>
		protected UInt16 ParamAsUInt16(
			string name,
			bool   isRequired,
			int    defValue )
		{
			try
			{
				return UInt16.Parse( ParamAsString(
					name,
					isRequired,
					String.Format("{0}", defValue)));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", name, thrown);
			}
		}

		
        /// <summary>
		/// Retrieves the value of the specified command line parameter as a
		/// 16 bit unsigned integer.
		/// </summary>
		/// <param name="longName">the full name of this parameter</param>
		/// <param name="shortName">the abbreviated name of this parameter</param>
		/// <param name="isRequired">whether or not the parameter MUST be specified</param>
		/// <param name="defValue">the default value of the parameter if not specified</param>
		/// <returns>the value of this parameter as a 16 bit unsigned integer</returns>
		protected UInt16 ParamAsUInt16(
			string longName,
			string shortName,
			bool   isRequired,
			int    defValue )
		{
			try
			{
				return UInt16.Parse( ParamAsString(
					longName,
					shortName,
					isRequired,
					String.Format("{0}", defValue)));
			}
			catch (FormatException thrown)
			{
				throw new ArgumentException("Bad format for parameter", longName, thrown);
			}
		}
	}
}
