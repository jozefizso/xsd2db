using System;

namespace Xsd2Db.CommandLineParser
{
	/// <summary>
	/// Summary description for Helper.
	/// </summary>
	internal sealed class Helper
	{
		/// <summary>
		/// Makes sure that the given parameter is not null and, if requested,
		/// not an empty string.
		/// </summary>
		/// <param name="paramName">the name of the parameter being validated</param>
		/// <param name="paramValue">the value of the parameter being validated</param>
		/// <exception cref="ArgumentNullException">
		///   If paramValue is null.
		/// </exception>
		public static void EnsureNotNull(
			string paramName,
			object paramValue)
		{
			if (paramValue == null)
			{
				throw new ArgumentNullException(
					String.Format("{0} may not be null", paramName),
					paramName);
			}
		}

		/// <summary>
		/// Makes sure that the given parameter is not null and, if requested,
		/// not an empty string.
		/// </summary>
		/// <param name="paramName">the name of the parameter being validated</param>
		/// <param name="paramValue">the value of the parameter being validated</param>
		/// <exception cref="ArgumentException">
		///   If paramValue is the empty string.
		/// </exception>
		public static void EnsureNotEmpty(
			string paramName,
			string paramValue)
		{
			EnsureNotNull(paramName, paramValue);

			if (String.Empty.Equals(paramValue))
			{
				throw new ArgumentException(
					String.Format("{0} may not be empty", paramName),
					paramName);
			}
		}
	}
}