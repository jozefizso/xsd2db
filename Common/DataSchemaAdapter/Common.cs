using System;

namespace Zks.Data.Common
{
	/// <summary>
	/// Intercase <c>IDataSchemaAdapter</c>
	/// </summary>
	public interface IDataSchemaAdapter
	{
		/// <summary>
		/// This method creates a new database based on XSD schema file.
		/// </summary>
		/// <param name="xmlSchemaFile">File name of XSD Schema.</param>
		void CreateDatabase( string xmlSchemaFile );
	}
}
