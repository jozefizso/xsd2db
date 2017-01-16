// For DataSet
	// For Trace
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
// For StringBuilder

namespace Xsd2Db.Data
{
	/// <summary>
	/// This abstract class implements most of the functionality required
	/// to map an XSD schema (represented with a DataSet) to an SQL creation
	/// script.
	/// </summary>
	public abstract class ScriptBasedDataSchemaAdapter : DataSchemaAdapter
	{
		private string tablePrefix;
		private string dbOwner;

		/// <summary>
		/// Create a new database conforming to the passed schema.
		/// </summary>
		/// <param name="schema">a DataSet containing the schema</param>
		/// <param name="force">overwrite the database if it exists</param>
		void DataSchemaAdapter.Create(DataSet schema, bool force)
		{
			Create(schema, force, string.Empty, "dbo");
		}

		public void Create(DataSet schema, bool force, string TablePrefix)
		{
			Create(schema, force, TablePrefix, "dbo");
		}

	    public void Create(DataSet schema, bool force, string TablePrefix, string DbOwner)
	    {
            Create(schema, force, TablePrefix, "dbo", true);
        }



        public void Create(DataSet schema, bool force, string TablePrefix, string DbOwner, bool useExisting)
		{
			this.TablePrefix = TablePrefix;
			this.DbOwner = DbOwner;

            //  IDbConnection oconn = new SqlConnection()
            if (!useExisting)
            {
                
                using (IDbConnection conn = GetConnection())
                {
                    conn.Open();

                    //
                    // Create the database if not use exisitng


                    string script = GetCreateScript(schema.DataSetName, force);
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        Debug.Assert(script.Length > 0);
                        cmd.CommandText = script;
                        cmd.ExecuteNonQuery();
                    }

                }
            }

            using (IDbConnection conn = GetConnection(schema.DataSetName))
			{
				conn.Open();

				//
				// Set up the schema
				//
				string script = GetSchemaScript(schema);
				if (script.Length > 0)
				{
					using (IDbCommand cmd = conn.CreateCommand())
					{
						cmd.CommandText = script;
						cmd.ExecuteNonQuery();
					}
				}
			}
		}


		/// <summary>
		/// Child classes should provice an implementation which returns a
		/// connection to the server on which to create the new database.
		/// </summary>
		/// <returns>an unopened connection to the server.</returns>
		protected abstract IDbConnection GetConnection();

		/// <summary>
		/// Child classes should provice an implementation which returns a
		/// connection to the server on which to create the new database.
		/// The returned connection must be bound to the context of the
		/// catalog/database given as a parameter (by name).
		/// </summary>
		/// <param name="catalog">The catalog (i.e., database) context
		/// to connect to</param>
		/// <returns>an unopened connection to the server.</returns>
		protected abstract IDbConnection GetConnection(string catalog);

		/// <summary>
		/// Returns the type description for the column given.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		protected abstract string GetTypeFor(DataColumn column);

		/// <summary>
		/// Returns a safe version of the given name.
		/// </summary>
		/// <param name="inputValue">Original Name</param>
		/// <returns>Converted name</returns>
		protected abstract string MakeSafe(string inputValue);

		/// <summary>
		/// Returns a script which creates an empty database having the given name.
		/// </summary>
		/// <param name="databaseName">the name of the database to create</param>
		/// <param name="overwrite">should an existing database (of the same name) be deleted</param>
		/// <returns>the script required to create an empty database having the given name</returns>
		internal string GetCreateScript(string databaseName, bool overwrite)
		{
			if ((databaseName == null) || (databaseName.Trim().Length == 0))
			{
				throw new ArgumentException(String.Format("The database name passed is {0}", ((databaseName == null) ? "null" : "empty")), "databaseName");
			}

			StringBuilder command = new StringBuilder();

			if (overwrite)
			{
				command.AppendFormat("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{0}') DROP DATABASE {1};\n CREATE DATABASE {1};\n", databaseName, MakeSafe(databaseName));
			}
			else
			{
				command.AppendFormat("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{0}') BEGIN USE {1}; END\n ELSE\n BEGIN\n CREATE DATABASE {1};\n END;\n", databaseName, MakeSafe(databaseName));
			}

			return command.ToString();
		}

		/// <summary>
		/// Returns the creation script which corresponds to the schema
		/// contained in the .xsd file which is passed as a parameter.
		/// </summary>
		/// <exception cref="ArgumentException">This method does not support
		/// the creation of a schema containing tables that have zero (0)
		/// columns.  Nor does is support relations where one (or both) sides
		/// of the relationship is defined by zero (0) columns.</exception>
		/// <exception cref="NotSupportedException">May be thrown if the
		/// method is unable to determine the database type for a column
		/// </exception>
		/// <param name="dataSet">the DataSet containing the database schema
		/// to be created.
		/// </param>
		/// <returns>An SQL creation script corresponding to the schema of
		/// the passed DataSet.</returns>
		internal string GetSchemaScript(DataSet dataSet)
		{
			if (dataSet == null)
			{
				throw new ArgumentException("null is not a valid parameter value", "dataSet");
			}

			StringBuilder command = new StringBuilder();

			foreach (DataTable table in dataSet.Tables)
				try
				{
					command.Append(MakeTable(table));
				}
				catch (ArgumentException exception)
				{
					throw new ArgumentException("Table does not contain any columns", table.TableName, exception);
				}

			foreach (DataTable table in dataSet.Tables)
				if (table.PrimaryKey.Length > 0)
				{
					string tableName = MakeSafe(tablePrefix + table.TableName);
					string primaryKeyName = MakeSafe("PK_" + tablePrefix + table.TableName);
					string primaryKeyList = MakeList(table.PrimaryKey);
					command.AppendFormat("ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT {1} PRIMARY KEY CLUSTERED ({2});\n", tableName, primaryKeyName, primaryKeyList);
				}

			foreach (DataRelation relation in dataSet.Relations)
				try
				{
					command.Append(MakeRelation(relation));
				}
				catch (ArgumentException exception)
				{
					throw new ArgumentException("Relationship has an empty column list", relation.RelationName, exception);
				}

			return command.ToString();
		}

		/// <summary>
		/// Returns a script which creates a database table that
		/// corresponds to <paramref name="table"/>.
		/// </summary>
		/// <param name="table"></param>
		/// <returns>the script which creates a table corresponding to <paramref name="table"/></returns>
		private string MakeTable(DataTable table)
		{
			StringBuilder command = new StringBuilder();

			string tableName = MakeSafe(tablePrefix + table.TableName);
			string tableColumns = MakeList(table.Columns);
			command.AppendFormat("if exists (select * from dbo.sysobjects where id = object_id(N'{0}') and OBJECTPROPERTY(id, N'IsUserTable') = 1) DROP TABLE {0};", tableName);
			command.AppendFormat("CREATE TABLE {0} ({1});\n", tableName, tableColumns);

			return command.ToString();
		}

		/// <summary>
		/// Returns the names of the columns in <paramref name="columns"/>.
		/// </summary>
		/// <param name="columns">the collection of columns to be put in the list</param>
		/// <returns>the names of the columns in a comma separated list</returns>
		/// <exception cref="System.ArgumentException">This is thrown if
		/// <paramref name="columns"/> is empty or null</exception>
		private string MakeList(DataColumn[] columns)
		{
			if ((columns == null) || (columns.Length < 1))
			{
				throw new ArgumentException("Invalid column list!", "columns");
			}

			StringBuilder list = new StringBuilder();
			bool isFirstColumn = true;
			foreach (DataColumn c in columns)
			{
				if (!isFirstColumn)
				{
					list.Append(", ");
				}

				list.Append(MakeSafe(c.ColumnName));

				isFirstColumn = false;
			}
			return list.ToString();
		}

		/// <summary>
		/// Returns the names of the columns in <paramref name="columns"/>.  Each
		/// name is followed by the type of data which the column contains.
		/// </summary>
		/// <param name="columns">the collection of columns to be put in the list</param>
		/// <returns>the names of the columns in a comma separated list</returns>
		/// <exception cref="System.ArgumentException">This is thrown if
		/// <paramref name="columns"/> is empty or null</exception>
		private string MakeList(DataColumnCollection columns)
		{
			if ((columns == null) || (columns.Count < 1))
			{
				throw new ArgumentException("Invalid column list!", "columns");
			}

			StringBuilder list = new StringBuilder();
			bool isFirstColumn = true;
			foreach (DataColumn c in columns)
			{
				if (!isFirstColumn)
				{
					list.Append(", ");
				}

				string columnName = MakeSafe(c.ColumnName);
				string columnType = GetTypeFor(c);
				list.AppendFormat("{0} {1}", columnName, columnType);

				isFirstColumn = false;
			}

			return list.ToString();
		}


		/// <summary>
		/// Returns a script which will create a database relations
		/// corresponding to the passed DataRelation.
		/// </summary>
		/// <param name="relation">the DataRelation to be scripted</param>
		/// <returns>the script to create the relation</returns>
		/// <exception cref="System.ArgumentException">This is thrown if
		/// <paramref name="relation"/> is null or any of the
		/// key sets in the relation are empty</exception>
		private string MakeRelation(DataRelation relation)
		{
			if (relation == null)
			{
				throw new ArgumentException("Invalid argument value (null)", "relation");
			}

			StringBuilder command = new StringBuilder();
			string childTable = MakeSafe(tablePrefix + relation.ChildTable.TableName);
			string parentTable = MakeSafe(tablePrefix + relation.ParentTable.TableName);
			string relationName = MakeSafe(relation.RelationName);
			string childColumns = MakeList(relation.ChildColumns);
			string parentColumns = MakeList(relation.ParentColumns);
			command.AppendFormat("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4});\n", childTable, relationName, childColumns, parentTable, parentColumns);
			return command.ToString();
		}

		public string TablePrefix
		{
			set { tablePrefix = value; }
			get { return tablePrefix; }
		}

		public string DbOwner
		{
			set { dbOwner = value; }
			get { return dbOwner; }
		}
	}
}