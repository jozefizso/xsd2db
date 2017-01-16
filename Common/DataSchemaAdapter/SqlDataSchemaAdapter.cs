using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Xsd2Db.Data
{
	/// <summary>
	/// A class which generates creation scripts which are compatible
	/// with Microsoft SQL Server 2000.
	/// </summary>
	public class SqlDataSchemaAdapter : ScriptBasedDataSchemaAdapter
	{
		/// <summary>
		/// A map between a data type name and a database type template
		/// </summary>
		internal static readonly Hashtable TypeMap;


		/// <summary>
		/// The database host to connect to.
		/// </summary>
		public readonly string Host;

		/// <summary>
		/// Creates a new SqlScriptGenerator instance.
		/// </summary>
		/// <param name="host">the database host to connect to</param>
		public SqlDataSchemaAdapter(string host)
		{
			this.Host = host;
		}

		/// <summary>
		/// 
		/// </summary>
		static SqlDataSchemaAdapter()
		{
			Hashtable typeMap = new Hashtable();
			typeMap[typeof (UInt64)] = "bigint {1}NULL";
			typeMap[typeof (Int64)] = "bigint {1}NULL";
			typeMap[typeof (Boolean)] = "bit {1}NULL";
			typeMap[typeof (Char)] = "char {1}NULL";
			typeMap[typeof (DateTime)] = "datetime {1}NULL";
			typeMap[typeof (Double)] = "float {1}NULL";
			typeMap[typeof (UInt32)] = "int {1}NULL";
			typeMap[typeof (Int32)] = "int {1}NULL";
			typeMap[typeof (Guid)] = "uniqueidentifier {1}NULL";
			typeMap[typeof (UInt16)] = "smallint {1}NULL";
			typeMap[typeof (Int16)] = "smallint {1}NULL";
			typeMap[typeof (Decimal)] = "real {1}NULL";
			typeMap[typeof (Byte)] = "tinyint {1}NULL";
			typeMap[typeof (String)] = "varchar({0}) {1}NULL";
			typeMap[typeof (TimeSpan)] = "int {1}NULL";
			typeMap[typeof (Byte[])] = "varbinary {1}NULL";

			SqlDataSchemaAdapter.TypeMap = typeMap;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override IDbConnection GetConnection()
		{
			return GetConnection(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="catalog"></param>
		/// <returns></returns>
		protected override IDbConnection GetConnection(string catalog)
		{
			StringBuilder connectionString = new StringBuilder();

			connectionString.AppendFormat(
				@"Data Source={0};Integrated Security=SSPI;Pooling='false';",
				this.Host);

			if ((catalog != null) && (catalog.Length != 0))
			{
				connectionString.AppendFormat(
					@"Initial Catalog='{0}';",
					catalog);
			}

			return new SqlConnection(connectionString.ToString());
		}

		/// <summary>
		/// SQL Server names to be more than 128 characters long.
		/// This function trims the given name and returns at
		/// most the first 128 characters.  It also wraps the name
		/// in square brackets (i.e., '[' and ']').
		/// </summary>
		/// <param name="inputValue">Original Name</param>
		/// <returns>Converted name</returns>
		protected override string MakeSafe(string inputValue)
		{
			String text = inputValue.Trim();
			return String.Format(
				"[{0}]",
				text.Substring(0, Math.Min(128, text.Length)));
		}

		/// <summary>
		/// Returns the type descriptor corresponding to
		/// <paramref name="column"/>.
		/// </summary>
		/// <param name="column">the DataColumn for which the type is desired</param>
		/// <returns>the type descriptor corresponding to
		/// <paramref name="column"/></returns>
		protected override string GetTypeFor(DataColumn column)
		{
			string template = (string) TypeMap[column.DataType];

			if ((column.DataType == typeof (String))
				&& (column.MaxLength < 0))
			{
				template = "text";
			}

			if (template == null)
			{
				throw new NotSupportedException(
					String.Format("No type mapping is provided for {0}",
					              column.DataType.Name));
			}

			return String.Format(
				template,
				column.MaxLength,
				(column.AllowDBNull ? String.Empty : "NOT "));
		}
	}
}