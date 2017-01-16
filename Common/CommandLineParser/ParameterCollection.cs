using System;
using System.Collections;

namespace Xsd2Db.CommandLineParser
{
	/// <summary>
	///   A type safe collection of parameters.  The collection can be
	///   accessed either by name or by positional index.
	/// </summary>
	public class ParameterCollection : CollectionBase
	{
		/// <summary>
		/// Create a new empty ParameterCollection object.
		/// </summary>
		internal ParameterCollection()
			: base()
		{
		}

		/// <summary>
		/// Sorts the parameter list using the IComparable interface
		/// of the Parameter instances.
		/// </summary>
		public void Sort()
		{
			this.InnerList.Sort();
		}

		/// <summary>
		/// Adds the specified parameter to this parameter collection.
		/// </summary>
		/// <param name="p">the parameter to add</param>
		/// <exception cref="ArgumentException">
		///   if a 
		/// </exception>
		public void Add(Parameter p)
		{
			Helper.EnsureNotNull("p", p);

			if (this.Contains(p))
			{
				throw new ArgumentException(String.Format(
					"Parameter collision! name = {0}" + (p.HasAlias ? " (alias={1})" : String.Empty),
					p.Name, p.Alias),
				                            "p");
			}

			this.InnerList.Add(p);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		///   The name of the parameter to find.
		/// </param>
		/// <returns>
		///   true if a parameter matching that name is in this collection
		/// </returns>
		public bool Contains(string name)
		{
			foreach (Parameter candidate in this.InnerList)
			{
				if (candidate.IsMatchFor(name)) return true;
			}

			return false;
		}

		/// <summary>
		///   Checks if the given parameter is a match for any parameter
		///   in this collection.  This is the case if the parameter's
		///   name or alias conflicts with the name or alias of a parameter
		///   already in the collection.
		/// </summary>
		/// <param name="p">the parameter for which to search</param>
		/// <returns>
		///   true if the given parameter is a match for any parameter
		///   in this collection.  This is the case if the parameter's
		///   name or alias conflicts with the name or alias of a parameter
		///   already in the collection.
		///</returns>
		public bool Contains(Parameter p)
		{
			foreach (Parameter candidate in this.InnerList)
			{
				if (candidate.IsMatchFor(p)) return true;
			}

			return false;
		}

		/// <summary>
		/// Retrieve a parameter by numerical index.
		/// </summary>
		/// <param name="i">the zero based index to retrieve</param>
		/// <returns>the parameter at the given index</returns>
		public Parameter Get(int i)
		{
			return (Parameter) this.InnerList[i];
		}

		/// <summary>
		/// Retrieve a parameter by name
		/// </summary>
		/// <param name="name">the name of the parameter to retrieve</param>
		/// <returns>the parameter with the given name</returns>
		/// <remarks>
		///   The name may be either the parameter name of its alias.  If
		///   the parameter is not case sensitive, the search will handle
		///   the comparison appropriately.
		/// </remarks>
		public Parameter Get(string name)
		{
			foreach (Parameter p in this.InnerList)
			{
				if (p.IsMatchFor(name))
				{
					return p;
				}
			}

			return null;
		}


		/// <summary>
		/// Retrieve a parameter by name.
		/// </summary>
		/// <remarks>
		///   The name may be either the parameter name of its alias.  If
		///   the parameter is not case sensitive, the search will handle
		///   the comparison appropriately.
		/// </remarks>
		public Parameter this[string name]
		{
			get { return Get(name); }
		}

		/// <summary>
		/// Retrieve a parameter by numerical index.
		/// </summary>
		public Parameter this[int i]
		{
			get { return Get(i); }
		}
	}
}