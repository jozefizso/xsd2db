using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ADOX;

namespace Xsd2Db.Data
{
	/// <summary>
	/// Summary description for JetDatabaseCreator.
	/// </summary>
	public abstract class AdoxDataSchemaAdapter : DataSchemaAdapter
	{
		/// <summary>
		/// 
		/// </summary>
		internal static readonly Hashtable TypeMap;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		protected abstract void DeleteCatalog(string name);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		protected abstract void CreateCatalog(string name);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected abstract Catalog OpenCatalog(string name);

		/// <summary>
		/// 
		/// </summary>
		static AdoxDataSchemaAdapter()
		{
			TypeMap = new Hashtable();

			TypeMap[typeof (UInt64)] = DataTypeEnum.adUnsignedBigInt;
			TypeMap[typeof (Int64)] = DataTypeEnum.adBigInt;
			TypeMap[typeof (Boolean)] = DataTypeEnum.adBoolean;
			TypeMap[typeof (Char)] = DataTypeEnum.adWChar;
			TypeMap[typeof (DateTime)] = DataTypeEnum.adDate;
			TypeMap[typeof (Double)] = DataTypeEnum.adDouble;
			TypeMap[typeof (UInt32)] = DataTypeEnum.adUnsignedInt;
			TypeMap[typeof (Int32)] = DataTypeEnum.adInteger;
			TypeMap[typeof (Guid)] = DataTypeEnum.adGUID;
			TypeMap[typeof (UInt16)] = DataTypeEnum.adUnsignedSmallInt;
			TypeMap[typeof (Int16)] = DataTypeEnum.adSmallInt;
			TypeMap[typeof (Decimal)] = DataTypeEnum.adDecimal;
			TypeMap[typeof (Byte)] = DataTypeEnum.adTinyInt;
			TypeMap[typeof (String)] = DataTypeEnum.adWChar;
			TypeMap[typeof (TimeSpan)] = DataTypeEnum.adDBTime;
			TypeMap[typeof (Byte[])] = DataTypeEnum.adLongVarBinary;
			TypeMap[typeof (Char[])] = DataTypeEnum.adLongVarWChar;
		}

		/// <summary>
		/// Create a new database conforming to the passed schema.
		/// </summary>
		/// <param name="schema">a DataSet containing the schema</param>
		/// <param name="force">overwrite the database if it exists</param>
		public void Create(
			DataSet schema,
			bool force)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema is null", "schema");
			}

			string name = schema.DataSetName;

			if (name.Equals(String.Empty))
			{
				throw new ArgumentException(
					"name of the schema (file name) not set",
					"schema.DataSetName");
			}

			if (force)
			{
				DeleteCatalog(name);
			}

			CreateCatalog(name);

			Catalog catalog = null;
			try
			{
				//Hashtable map = new Hashtable();

				catalog = OpenCatalog(name);

				foreach (DataTable srcTable in schema.Tables)
				{
					CreateTable(catalog, srcTable);
				}

				foreach (DataRelation relation in schema.Relations)
				{
					CreateRelation(catalog, relation);
				}
			}
			finally
			{
				//
				// TODO: is this still required.
				//

				// Force the system to let go of the connection, otherwise
				// it keeps a handle to the database file.
				if (catalog != null)
				{
					catalog.ActiveConnection = null;
					catalog = null;
				}
				GC.Collect();
			}
		}

		/// <summary>
		/// Create a new database conforming to the passed schema.
		/// </summary>
		/// <param name="schema">a DataSet containing the schema</param>
		/// <param name="force">overwrite the database if it exists</param>
		/// <param name="TablePrefix">Adds a prefix to all tables</param>
		public void Create(DataSet schema, bool force, string TablePrefix)
		{
			Create(schema, force);
		}

		/// <summary>
		/// Create a new database conforming to the passed schema.
		/// </summary>
		/// <param name="schema">a DataSet containing the schema</param>
		/// <param name="force">overwrite the database if it exists</param>
		/// <param name="TablePrefix">Adds a prefix to all tables</param>
		/// <param name="DbOwner">Not Implemented yet</param>
		public void Create(DataSet schema, bool force, string TablePrefix, string DbOwner)
		{
			Create(schema, force);
		}

	    public void Create(DataSet schema, bool force, string TablePrefix, string DbOwner, bool useExisting)
	    {
            Create(schema, force);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="srcTable"></param>
        internal void CreateTable(
			Catalog catalog,
			DataTable srcTable)
		{
			string tableName = Sanitize(srcTable.TableName);
			Table newTable = new TableClass();
			newTable.Name = tableName;
			catalog.Tables.Append(newTable);

			// ArrayList keySet = new ArrayList();

			foreach (DataColumn srcColumn in srcTable.Columns)
			{
				Column column = new ColumnClass();

				column.Name = Sanitize(srcColumn.ColumnName);
				column.Type = TypeFor(srcColumn);
				column.DefinedSize = SizeFor(srcColumn);
				column.ParentCatalog = catalog;

				if (srcColumn.AllowDBNull)
				{
					column.Attributes = ColumnAttributesEnum.adColNullable;
				}

				LookupTable(catalog, srcTable).Columns.Append(
					column,
					DataTypeEnum.adVarWChar, // default
					0); // default
			}

			if (srcTable.PrimaryKey.Length > 0)
			{
				Key key = new KeyClass();
				key.Name = Sanitize(String.Format("{0}", tableName));
				key.Type = KeyTypeEnum.adKeyPrimary;
				key.RelatedTable = tableName;
				foreach (DataColumn srcColumn in srcTable.PrimaryKey)
				{
					Column column = LookupColumn(catalog, srcColumn);
					key.Columns.Append(
						column.Name,
						DataTypeEnum.adVarWChar, // default
						0); // default
				}

				LookupTable(catalog, srcTable).Keys.Append(
					key,
					KeyTypeEnum.adKeyPrimary, // default
					Type.Missing, // default
					String.Empty, // default
					String.Empty); // default
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="catalog"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		internal Column LookupColumn(
			Catalog catalog,
			DataColumn column)
		{
			string columnName = Sanitize(column.ColumnName);

			Column result
				= LookupTable(catalog, column.Table).Columns[columnName];

			Debug.Assert(result != null);

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="catalog"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		internal Table LookupTable(
			Catalog catalog,
			DataTable table)
		{
			string tableName = Sanitize(table.TableName);

			Table result = catalog.Tables[tableName];

			Debug.Assert(result != null);

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="catalog"></param>
		/// <param name="relation"></param>
		internal void CreateRelation(
			Catalog catalog,
			DataRelation relation)
		{
			Key foreignKey = new KeyClass();
			Table parentTable = LookupTable(catalog, relation.ParentTable);
			Table childTable = LookupTable(catalog, relation.ChildTable);

			foreignKey.Name = Sanitize(relation.RelationName);
			foreignKey.Type = KeyTypeEnum.adKeyForeign;
			foreignKey.RelatedTable = parentTable.Name;
			foreignKey.DeleteRule = RuleEnum.adRICascade;
			foreignKey.UpdateRule = RuleEnum.adRINone;

			//
			// Assumption, child and parent columns are at the same index
			// in their respective collections.
			//

			Debug.Assert(
				relation.ChildColumns.Length == relation.ParentColumns.Length);

			for (int i = 0; i < relation.ChildColumns.Length; ++i)
			{
				Column childColumn
					= LookupColumn(catalog, relation.ChildColumns[i]);
				Column parentColumn
					= LookupColumn(catalog, relation.ParentColumns[i]);

				foreignKey.Columns.Append(
					childColumn.Name,
					DataTypeEnum.adVarWChar, // default
					0); // default

				foreignKey.Columns[childColumn.Name].RelatedColumn
					= parentColumn.Name;
			}

			childTable.Keys.Append(
				foreignKey,
				KeyTypeEnum.adKeyPrimary, // default
				Type.Missing, // default
				String.Empty, // default
				String.Empty); // default
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		internal Type ResolveType(DataColumn column)
		{
			Type type = column.DataType;

			if (type.Equals(typeof (string)) && (column.MaxLength < 0))
			{
				type = typeof (char[]);
			}

			return type;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		internal DataTypeEnum TypeFor(DataColumn column)
		{
			Type type = ResolveType(column);

			object result = TypeMap[type];

			Debug.Assert(result != null);

			return (DataTypeEnum) result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		internal int SizeFor(DataColumn column)
		{
			//
			// TODO: Maybe I should just return the max length for strings
			// and 0 (which means "use the default" for everything else?
			//

			Type type = ResolveType(column);

			if (type.Equals(typeof (char[])) || type.Equals(typeof (byte[])))
			{
				return 0;
			}
			else if (type.Equals(typeof (string)))
			{
				return column.MaxLength;
			}
			else if (type.Equals(typeof (DateTime)))
			{
				return 0;
			}
			else
			{
				return Marshal.SizeOf(type);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		internal string Sanitize(string text)
		{
			return text.Trim().ToLower().Replace("-", "_").Replace(" ", "_");
		}

	}
}