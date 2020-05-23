// File delete
	// For StringBuilder
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Xsd2Db.Data.Test
{
	/// <summary>
	/// This class contains tests that involve a database connection.
	/// </summary>
	[TestFixture]
	public class SqlTest : Common
	{
		/// <summary>
		/// 
		/// </summary>
		public SqlTest()
			: base("sql_test")
		{
			string host = ConfigurationManager.AppSettings["Test.SqlHost"];
			if (!String.IsNullOrEmpty(host))
			{
				this.Host = host;
			}
		}

		/// <summary>
		/// The database host on which all of the tests will be performed.
		/// </summary>
		public string Host = @".\SQLExpress";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="commandText"></param>
		public override void ValidateExceptionOnDuplicateInsert(Exception exception, string commandText)
		{
			SqlException theException = exception as SqlException;

			Assertion.AssertNotNull(String.Format("Expected an SQL Exception for '{0}'!  Received {1} '{2}'", commandText, exception.GetType(), exception.Message), theException);

			Assertion.Assert(String.Format("Unexpected Exception!\r\n\tException : '{0}'\r\n\tFor Query : '{1}'", theException.Message, commandText), theException.Number == 2627);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="commandText"></param>
		public override void ValidateExceptionOnBadForeignKeyInsert(Exception exception, string commandText)
		{
			SqlException theException = exception as SqlException;

			Assertion.AssertNotNull(String.Format("Expected an SQL Exception for '{0}'!  Received {1} '{2}'", commandText, exception.GetType(), exception.Message), theException);

			Assertion.Assert(String.Format("Unexpected Exception!\r\n\tException : '{0}'\r\n\tFor Query : '{1}'", theException.Message, commandText), theException.Number == 547);
		}


		/// <summary>
		/// Returns a properly formatted connection string.
		/// </summary>
		/// <param name="host">the database server host</param>
		/// <param name="catalog">the database to connect to by default</param>
		/// <returns>A properly formatted connection string</returns>
		public IDbConnection Connection(string host, string catalog)
		{
			StringBuilder connectionString = new StringBuilder();

			connectionString.AppendFormat(@"Data Source={0};Integrated Security=SSPI;Pooling='false';", host);

			if ((catalog != null) && (catalog.Length != 0))
			{
				connectionString.AppendFormat(@"Initial Catalog='{0}';", catalog);
			}

			return new SqlConnection(connectionString.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IDbConnection Connection()
		{
			return Connection(this.Host, this.Catalog);
		}


		/// <summary>
		/// Test to see what happens if the user tries to get a database
		/// creation script for a null database name.
		/// </summary>
		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestNullName()
		{
			string name = null;
			string s = new SqlDataSchemaAdapter(Host).GetCreateScript(name, false);
		}

		/// <summary>
		/// Test to see what happens if the user tries to get a database
		/// creation script for a zero-length database name.
		/// </summary>
		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestEmptyName()
		{
			string name = "";
			string s = new SqlDataSchemaAdapter(Host).GetCreateScript(name, false);
		}

		/// <summary>
		/// Test to see what happens if the user tries to get a database
		/// creation script for a database name that doesn't have any
		/// non-whitespace characters.
		/// </summary>
		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestContentFreeName()
		{
			string name = "    ";
			string s = new SqlDataSchemaAdapter(Host).GetCreateScript(name, false);
		}

		/// <summary>
		/// Test to see what happens if the user tries to create a schema
		/// definition script for a null data set.
		/// </summary>
		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestNullDataSet()
		{
			DataSet ds = null;
			string s = new SqlDataSchemaAdapter(Host).GetSchemaScript(ds);
		}

		/// <summary>
		/// Test to see what happens if the user tries to get a schema
		/// creation script for a dataSet containing a table that has
		/// no columns.
		/// </summary>
		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestNoColumns1()
		{
			DataSet ds = new DataSet("TestNoColumns");
			ds.Tables.Add("empty_table");

			string s = new SqlDataSchemaAdapter(Host).GetSchemaScript(ds);
		}

		/// <summary>
		/// Test to see what happens if the user tries to get a schema
		/// creation script for a dataSet containing a table that has
		/// no columns.  The schema is loaded from a file.
		/// </summary>
		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestNoColumns2()
		{
			string name = "TestNoColumns2";
			string file = Path.GetFullPath(Path.ChangeExtension(name, ".xsd"));
			try
			{
				DataSet ds = new DataSet(name);
				ds.Tables.Add("empty_table_1");
				ds.Tables.Add("empty_table_2");
				ds.WriteXmlSchema(file);
				ds.Dispose();
				ds = new DataSet();
				ds.ReadXmlSchema(file);
				string s = new SqlDataSchemaAdapter(Host).GetSchemaScript(ds);
			}
			finally
			{
				File.Delete(file);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="force">should the database be overwritten</param>
		public void CreateEmptyDB(bool force)
		{
			Helper.Print("\n---\n--- Starting CreateEmptyDB\n---\n");

			DataSchemaAdapter creator = new SqlDataSchemaAdapter(Host);
			DataSet dataSet = new DataSet(this.Catalog);

			creator.Create(dataSet, force);

			//
			// Make sure that it has been created
			//
			using (IDbConnection connection = Connection())
			{
				connection.Open();
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = String.Format("select count(*) from master.dbo.sysdatabases WHERE name = N'{0}'", this.Catalog);
					Assertion.AssertEquals("Wrong number of databases found", 1, (int) command.ExecuteScalar());
				}
			}
		}

		/// <summary>
		/// Tests the creation of an empty database.
		/// </summary>
		[Test]
		public void CreateEmptyDB()
		{
			CreateEmptyDB(false);
		}

		/// <summary>
		/// Should fail to create a duplicate database without the force
		/// flag being  set.
		/// </summary>
		[Test]
		public void FailOnDuplicate()
		{
			CreateEmptyDB(false);
			try
			{
				CreateEmptyDB(false);
				Assertion.Assert("Created duplicate database without cleanup!", false);
			}
			catch (SqlException e)
			{
				Assertion.Assert("Wrong SQL exception (" + e.Number + ")", e.Number == 0x0709);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void OverwriteDB()
		{
			CreateEmptyDB(false);
			CreateEmptyDB(true);
		}

		/// <summary>
		/// Perform some basic insertion tests using primary and foreign
		/// key constraints.
		/// </summary>
		[Test]
		public void RelationTest1()
		{
			Helper.Print("\n---\n--- Starting RelationTest1\n---\n");
			DataSet ds = base.MakeDataSet();
			base.DoRelationTest(ds, new SqlDataSchemaAdapter(this.Host));
		}

		/// <summary>
		/// Perform some basic insertion tests using primary and foreign
		/// key constraints.
		/// </summary>
		[Test]
		public void RelationTest2()
		{
			Helper.Print("\n---\n--- Starting RelationTest2\n---\n");

			string file = "RelationTestFromFile.xsd";

			using (DataSet ds = base.MakeDataSet())
			{
				ds.WriteXmlSchema(file);
				ds.Dispose();
			}

			using (DataSet ds = new DataSet())
			{
				ds.ReadXmlSchema(file);
				base.DoRelationTest(ds, new SqlDataSchemaAdapter(this.Host));
			}
		}


		/// <summary>
		/// Tests the creation (and subsequent removal) of a database. Test with a complex XSD file
		/// </summary>
		[Test]
		public void CreateFromOntology()
		{
			Helper.Print("\n---\n--- Starting CreateFromOntology\n---\n");

			DataSchemaAdapter creator = new SqlDataSchemaAdapter(this.Host);
			DataSet dataSet = new DataSet(this.Catalog);

			NameValueCollection config = (NameValueCollection) ConfigurationSettings.GetConfig("Xsd2Db.Data.Test");
			string xsdFile = null;
			if (config != null)
			{
				xsdFile = config["ObjectModelXsd"];
			}
			if (xsdFile == null || xsdFile.Length == 0)
				xsdFile = "../../Common/DataSchemaAdapter/Test/ObjectModel.xsd";

			dataSet.ReadXmlSchema(xsdFile);
			dataSet.DataSetName = this.Catalog;

			creator.Create(dataSet, true);

			//
			// Make sure that it has been created
			//
			using (IDbConnection connection = Connection())
			{
				connection.Open();
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = String.Format("select count(*) from master.dbo.sysdatabases WHERE name = N'{0}'", this.Catalog);
					Assertion.AssertEquals("Wrong number of databases found", 1, (int) command.ExecuteScalar());
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			using (IDbConnection connection = Connection(Host, null))
			{
				connection.Open();

				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = String.Format("select count(*) from master.dbo.sysdatabases WHERE name = N'{0}'", this.Catalog);
					Assertion.AssertEquals("Wrong number of databases found", 0, (int) command.ExecuteScalar());
				}
			}
		}

		/// <summary>
		/// Cleanup code to ensure that the tests don't leave anything
		/// behind in the database.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			using (IDbConnection connection = Connection(Host, null))
			{
				connection.Open();

				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = String.Format("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{0}') DROP DATABASE [{0}];\n", Catalog);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}