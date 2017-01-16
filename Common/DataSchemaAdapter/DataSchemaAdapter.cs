using System.Data;

namespace Xsd2Db.Data
{
	/// <summary>
	/// Summary description for DataSchemaAdapter.
	/// </summary>
	public interface DataSchemaAdapter
	{
		/// <summary>
		/// Create a new database conforming to the passed schema.
		/// </summary>
		/// <param name="schema">a DataSet containing the schema</param>
		/// <param name="force">overwrite the database if it exists</param>
		void Create(
			DataSet schema,
			bool force);

		/// <summary>
		/// Create a new database conforming to the passed schema.
		/// </summary>
		/// <param name="schema">a DataSet containing the schema</param>
		/// <param name="force">overwrite the database if it exists</param>
		/// <param name="TablePrefix">Adds a prefix to all tables</param>
		void Create(
			DataSet schema,
			bool force,
			string TablePrefix
			);

		/// <summary>
		/// Create a new database conforming to the passed schema.
		/// </summary>
		/// <param name="schema">a DataSet containing the schema</param>
		/// <param name="force">overwrite the database if it exists</param>
		/// <param name="TablePrefix">Adds a prefix to all tables</param>
		/// <param name="DbOwner">specifies a DBOwner if supported to the tables</param>
		void Create(
			DataSet schema,
			bool force,
			string TablePrefix,
			string DbOwner
			);
        void Create(
             DataSet schema,
             bool force,
             string TablePrefix,
             string DbOwner, 
             bool useExisitng
             );
    }
}