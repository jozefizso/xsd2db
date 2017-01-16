# if FALSE
// File delete
	// For StringBuilder
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using NUnit.Framework;

namespace Xsd2Db.Data.SqlClient.Test
{
	internal class Settings
	{
		public static SqlConnection Connection;
		public static string ConnectionString = ((NameValueCollection) ConfigurationSettings.GetConfig("Xsd2Db.Data.SqlClient.Test"))["ConnectionString"];
	}

	/// <summary>
	/// Tests the database connection. If this test fails, most of other tests will do too
	/// </summary>
	[TestFixture]
	public class SqlDataSchemaAdapterTestConnection
	{
		[SetUp]
		public void Start()
		{
			try
			{
				Settings.Connection = new SqlConnection(Settings.ConnectionString);
				Settings.Connection.Open(); // this conneciton will be closed in TeardownTestCreate
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		[Test]
		public void Run()
		{
			SqlCommand cmd = new SqlCommand("CREATE DATABASE ConnectionTest", Settings.Connection);
			cmd.ExecuteNonQuery();

			SqlConnection conn = new SqlConnection(Settings.ConnectionString + ";Initial Catalog=ConnectionTest");
			conn.Open();
			SqlCommand command = new SqlCommand("select name from master.dbo.sysdatabases WHERE name = N'ConnectionTest'", conn);
			command.ExecuteScalar();
			conn.Close();
		}

		[TearDown]
		public void Stop()
		{
			SqlCommand cmd = new SqlCommand("DROP DATABASE ConnectionTest", Settings.Connection);
			cmd.ExecuteNonQuery();

			Settings.Connection.Close();
		}
	}


	/// <summary>
	/// Tests passing invalid arguments such as null references, etc.
	/// </summary>
	[TestFixture]
	public class SqlDataSchemaAdapterTestInvalidArguments
	{
		[Test]
		[ExpectedException(typeof (FileNotFoundException))]
		public void TestFileNotFound()
		{
			Settings.Connection = new SqlConnection(Settings.ConnectionString);
			Settings.Connection.Open();

			SqlDataSchemaAdapter adapter = new SqlDataSchemaAdapter(Settings.Connection);

			adapter.CreateDatabase("file_not_found.xsd");
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestNullConnection()
		{
			Settings.Connection = null;

			SqlDataSchemaAdapter adapter = new SqlDataSchemaAdapter(Settings.Connection);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestInvalidConnection()
		{
			Settings.Connection = new SqlConnection(Settings.ConnectionString);

			SqlDataSchemaAdapter adapter = new SqlDataSchemaAdapter(Settings.Connection);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void TestNoColumns()
		{
			//
			// Check to see if we have a connection to the database
			// 
			Settings.Connection.Open();
			SqlDataSchemaAdapter adapter = new SqlDataSchemaAdapter(Settings.Connection);
			//
			// Just making sure that the server doesn't already have a Test-Database
			//
			new SqlCommand("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Test-Database') DROP DATABASE [Test-Database]",
			               Settings.Connection)
				.ExecuteNonQuery();

			DataSet ds = new DataSet("Test-Database");
			DataTable t = ds.Tables.Add();
			DataTable t2 = ds.Tables.Add();

			ds.WriteXmlSchema("test.xsd");

			adapter.CreateDatabase("test.xsd");
		}
	}


	/// <summary>
	/// Tests creation of relations such as primary keys and foreign keys
	/// </summary>
	[TestFixture]
	public class SqlDataSchemaAdapterTestRelations
	{
		[SetUp]
		public void SetupTestCreate()
		{
			Settings.Connection = new SqlConnection(Settings.ConnectionString);
			Settings.Connection.Open();

			try
			{
				//
				// Just making sure that the server doesn't already have a Test-Database
				//
				new SqlCommand("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Test-Database') DROP DATABASE [Test-Database]",
				               Settings.Connection)
					.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				throw;
			}
		}

		[Test]
		[ReflectionPermission(SecurityAction.LinkDemand, Unrestricted=true)]
		public void TestRelations()
		{
			// 
			// Setup a simple foreign key relation
			// 
			DataSet ds = new DataSet("Test-Database");
			DataTable master = ds.Tables.Add("Master");
			DataColumn mid = master.Columns.Add("MasterID", Type.GetType("System.Guid"));
			DataColumn[] keys = new DataColumn[1];
			keys[0] = mid;
			master.PrimaryKey = keys;
			DataColumn uniqueCol = master.Columns.Add("UniqueText", Type.GetType("System.String"));
			uniqueCol.Unique = true;
			uniqueCol.AllowDBNull = true;
			uniqueCol.MaxLength = 80;

			DataTable detail = ds.Tables.Add("Detail");
			DataColumn did = detail.Columns.Add("DetailID", Type.GetType("System.Guid"));
			DataColumn mid_fk = detail.Columns.Add("MasterID_FK", Type.GetType("System.Guid"));
			DataColumn[] keys2 = new DataColumn[1];
			keys2[0] = did;
			detail.PrimaryKey = keys2;

			ds.Relations.Add(mid, mid_fk);

			// 
			// Write out the XSD
			// 
			ds.WriteXmlSchema("test.xsd");

			// 
			// Create a database
			// 
			SqlDataSchemaAdapter adapter = new SqlDataSchemaAdapter(Settings.Connection);
			adapter.CreateDatabase("test.xsd");

			//  
			//  Check to see if SqlDataSchemaAdapter have created primary relations
			// 
			SqlCommand insertCommand = new SqlCommand("USE [Test-Database] INSERT Master (MasterID) VALUES ('{37477C57-447D-4077-B96D-FE49D79045AA}')",
			                                          Settings.Connection);

			int i = insertCommand.ExecuteNonQuery();
			Assertion.Assert("Insert query failed. Expect 1 row affected", i == 1);

			// Try executing the same insert once again
			bool raisedSqlException = false;
			try
			{
				insertCommand.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				Assertion.Assert("Primary key constraint failed. Incompatible SqlException '" + ex.Message + "'", ex.Number == 2627);
				raisedSqlException = true;
			}
			Assertion.Assert("Primary key constraint failed. Expected SqlException", raisedSqlException);

			//  
			//  Check to see if SqlDataSchemaAdapter have created the foreign key relations
			// 
			insertCommand = new SqlCommand("USE [Test-Database] INSERT Detail (DetailID, MasterID_FK) VALUES ('{37477C58-447D-4077-B96D-FE49D79045AA}', '{37477C57-447D-4077-B96D-FE49D79045AA}')",
			                               Settings.Connection);

			i = insertCommand.ExecuteNonQuery();
			Assertion.Assert("Insert query failed. Expect 1 row affected", i == 1);

			// Try executing the same insert once again
			raisedSqlException = false;
			try
			{
				insertCommand.CommandText = "USE [Test-Database] INSERT Detail (DetailID, MasterID_FK) VALUES ('{37477C59-447D-4077-B96D-FE49D79045AA}', '{DEADBEEF-447D-4077-B96D-FE49D79045AA}')";
				insertCommand.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				Assertion.Assert("Primary key constraint failed. Incompatible SqlException '" + ex.Message + "'", ex.Number == 547);
				raisedSqlException = true;
			}
			Assertion.Assert("Foreign key constraint failed. Expected SqlException", raisedSqlException);
		}


		[TearDown]
		public void TeardownTestCreate()
		{
			File.Delete("test.xsd");

			// release connection to avoid locking the database
			Settings.Connection.Close();
			Settings.Connection.Open();

			new SqlCommand("DROP DATABASE [Test-Database]",
			               Settings.Connection)
				.ExecuteNonQuery();
			Settings.Connection.Close();
		}
	}


	/// <summary>
	/// Test unusual cases of object naming
	/// </summary>
	[TestFixture]
	[ReflectionPermission(SecurityAction.LinkDemand, Unrestricted=true)]
	public class SqlDataSchemaAdapterTestNaming
	{
		[SetUp]
		public void SetupTestCreate()
		{
			try
			{
				DataSet ds = new DataSet("Test-Database");
				DataTable t = ds.Tables.Add();
				t.TableName = "TestTableOrders";
				DataColumn pid1 = t.Columns.Add("OrderID", Type.GetType("System.Guid"));
				DataColumn[] keys = new DataColumn[1];
				keys[0] = pid1;
				t.PrimaryKey = keys;

				DataColumn id1 = t.Columns.Add("CustomerID_FK", Type.GetType("System.Guid"));
				DataColumn str = t.Columns.Add("Nameeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
				                               Type.GetType("System.String"));
				str.MaxLength = 256;
				DataColumn txt = t.Columns.Add("SomeText", Type.GetType("System.String"));

				DataColumn int16 = t.Columns.Add("Some-Wired_Value--Name", Type.GetType("System.Int16"));
				int16.AllowDBNull = true;

				DataTable t2 = ds.Tables.Add();
				t2.TableName = "TestTable-Customers";
				DataColumn id2 = t2.Columns.Add("CustomerID", Type.GetType("System.Guid"));
				keys = new DataColumn[1];
				keys[0] = id2;
				t2.PrimaryKey = keys;

				ds.Relations.Add(id2, id1);

				ds.WriteXmlSchema("test.xsd");

				Settings.Connection = new SqlConnection(Settings.ConnectionString);
				Settings.Connection.Open(); // this conneciton will be closed in TeardownTestCreate

				/*
				* Just making sure that the server doesn't already have a Test-Database
				*/
				new SqlCommand("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Test-Database') DROP DATABASE [Test-Database]",
				               Settings.Connection)
					.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				throw;
			}
		}

		[Test]
		public void TestCreate()
		{
			SqlConnection tempConn = null;
			try
			{
				// Create a listener, which outputs to the console screen, and 
				// add it to the trace listeners.
				// Trace.Listeners.Add( new TextWriterTraceListener(System.Console.Out) );

				/*
				 * Check to see if we have a connection to the database
				 */
				SqlDataSchemaAdapter adapter = new SqlDataSchemaAdapter(Settings.Connection);

				adapter.CreateDatabase("test.xsd");
				/*
				 * Assure hte database exists
				 */
				tempConn = new SqlConnection(Settings.ConnectionString + ";Initial Catalog=Test-Database");

				tempConn.Open();
				SqlCommand cmd = new SqlCommand("SELECT count(name) FROM sysobjects where name = 'TestTableOrders'", tempConn);
				int orders = (int) cmd.ExecuteScalar();
				cmd = new SqlCommand("SELECT count(name)  FROM sysobjects where name = 'TestTable-Customers'", tempConn);
				int customers = (int) cmd.ExecuteScalar();
				tempConn.Close();

				Assertion.Assert("Table 'TestTableOrders' wasn't created", orders == 1);
				Assertion.Assert("Table 'TestTable-Customers' wasn't created", customers == 1);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				throw;
			}
			finally
			{
				if (tempConn != null)
					tempConn.Close();
			}
		}

		[TearDown]
		public void TeardownTestCreate()
		{
			File.Delete("test.xsd");

			// release connection to avoid locking the database
			Settings.Connection.Close();
			Settings.Connection.Open();

			/* 
			* drop the Test_Database database
			*/
			new SqlCommand("DROP DATABASE [Test-Database]",
			               Settings.Connection)
				.ExecuteNonQuery();
		}
	}
}

#endif