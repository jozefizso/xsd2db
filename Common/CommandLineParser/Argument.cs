namespace Xsd2Db.CommandLineParser
{
	/// <summary>
	/// Summary description for Argument.
	/// </summary>
	public class Argument
	{
		/// <summary>
		/// Private constructor for an argument record.
		/// </summary>
		/// <param name="name">the name of the argument</param>
		/// <param name="text">the value of the argument</param>
		internal Argument(string name, string text)
		{
			this.Name = name;
			this.Value = text;
		}

		/// <summary>
		/// The name of this argument.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The value of this argument
		/// </summary>
		public readonly string Value;
	}

}