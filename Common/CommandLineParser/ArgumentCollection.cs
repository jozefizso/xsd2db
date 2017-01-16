using System;
using System.Collections;

namespace Xsd2Db.CommandLineParser
{
	/// <summary>
	/// A type safe collection of Arguments are stored in the order in
	/// which they appear on the command line.  The program name is not
	/// included in an ArgumentCollection.
	/// </summary>
	public class ArgumentCollection : ReadOnlyCollectionBase
	{
		/// <summary>
		/// Internal constructor.  An argument collection can only be
		/// created within this assembly.
		/// </summary>
		internal ArgumentCollection()
			: base()
		{
		}

		/// <summary>
		/// Removes all arguments from the collection.  An argument
		/// collection can only be cleared within this assembly.
		/// </summary>
		internal void Clear()
		{
			this.InnerList.Clear();
		}

		/// <summary>
		/// Adds an argument to the argument list.  An argument collection
		/// is only modifiable within this assembly.
		/// </summary>
		/// <param name="argument">the argument to be appended</param>
		internal void Add(Argument argument)
		{
			this.InnerList.Add(argument);
		}

		/// <summary>
		/// Adds an argument to the argument list.  An argument collection
		/// is only modifiable within this assembly.
		/// </summary>
		/// <param name="name">the name of the argument</param>
		/// <param name="text">the value of the argument</param>
		internal void Add(String name, string text)
		{
			Add(new Argument(name, text));
		}

		/// <summary>
		/// Get an argument by positional index.  Arguments are stored in
		/// the order in which they appear on the command line.  The program
		/// name is in this collection.
		/// </summary>
		public Argument this[int i]
		{
			get { return (Argument) this.InnerList[i]; }
		}
	}
}