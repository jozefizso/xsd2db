// File delete
	// For StringBuilder
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Security.Permissions;
using NUnit.Framework;

namespace Xsd2Db.Data.Test
{
	/// <summary>
	/// This class contains tests that involve a database connection.
	/// </summary>
	[TestFixture]
	public class JetTest : Common
	{
		/// <summary>
		/// 
		/// </summary>
		public JetTest()
			: base("jet_test")
		{
		}

		/// <summary>
		/// Returns a properly formatted connection string.
		/// </summary>
		/// <returns>A properly formatted connection string</returns>
		public override IDbConnection Connection()
		{
			return new OleDbConnection(JetDataSchemaAdapter.GetConnectionString(Catalog));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="commandText"></param>
		public override void ValidateExceptionOnDuplicateInsert(Exception exception, string commandText)
		{
			OleDbException theException = exception as OleDbException;

			Assertion.AssertNotNull(String.Format("Expected an OLE DB Exception for '{0}'!  Received {1} '{2}'", commandText, exception.GetType(), exception.Message), theException);

			Assertion.Assert(String.Format("Unexpected Exception!\r\n\tException : '{0}'\r\n\tFor Query : '{1}'", theException.Message, commandText), theException.ErrorCode == -2147467259);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="commandText"></param>
		public override void ValidateExceptionOnBadForeignKeyInsert(Exception exception, string commandText)
		{
			OleDbException theException = exception as OleDbException;

			Assertion.AssertNotNull(String.Format("Expected an OLEDB Exception for '{0}'!  Received {1} '{2}'", commandText, exception.GetType(), exception.Message), theException);

			Assertion.Assert(String.Format("Unexpected Exception!\r\n\tException : '{0}'\r\n\tFor Query : '{1}'", theException.Message, commandText), theException.ErrorCode == -2147467259);
		}

		/// <summary>
		/// Test to see what happens if the user tries to create a schema
		/// definition script for a null data set.
		/// </summary>
		[Test]
		public void TestNullDataSet()
		{
			DataSchemaAdapter creator = new JetDataSchemaAdapter();
			Assert.Throws<ArgumentNullException>(() => creator.Create((DataSet) null, false));
		}

		/// <summary>
		/// Test to see what happens if the user tries to create a schema
		/// definition script for an unnamed data set.
		/// </summary>
		[Test]
		public void TestUnnamedDataSet()
		{
			DataSchemaAdapter creator = new JetDataSchemaAdapter();
			Assert.Throws<ArgumentException>(() => creator.Create(new DataSet(String.Empty), false));
		}

		/// <summary>
		/// Test to see what happens if the user tries to get a schema
		/// creation script for a dataSet containing a table that has
		/// no columns.
		/// </summary>
		[Test]
		public void TestNoColumns1()
		{
			DataSet ds = new DataSet(Catalog);
			ds.Tables.Add("empty_table");

			DataSchemaAdapter creator = new JetDataSchemaAdapter();
			creator.Create(ds, false);
		}

		/// <summary>
		/// Test to see what happens if the user tries to get a schema
		/// creation script for a dataSet containing a table that has
		/// no columns.  The schema is loaded from a file.
		/// </summary>
		[Test]
		public void TestNoColumns2()
		{
			string name = "TestNoColumns2";
			string file = String.Format("{0}.xsd", name);
			DataSet ds = new DataSet(Catalog);
			ds.Tables.Add("empty_table_1");
			ds.Tables.Add("empty_table_2");
			ds.WriteXmlSchema(file);
			ds.Dispose();
			ds = new DataSet();
			ds.ReadXmlSchema(file);
			DataSchemaAdapter creator = new JetDataSchemaAdapter();
			creator.Create(ds, false);
		}

		/// <summary>
		/// Tests the creation (and subsequent removal) of a database.
		/// </summary>
		[Test]
		public void CreateEmptyDB()
		{
			Helper.Print("\n---\n--- Starting CreateEmptyDB\n---\n");

			DataSchemaAdapter creator = new JetDataSchemaAdapter();
			DataSet dataSet = new DataSet(this.Catalog);

			creator.Create(dataSet, true);

			Assertion.Assert(File.Exists(JetDataSchemaAdapter.GetPath(this.Catalog)));

			//
			// Can we open it?
			//
			using (IDbConnection connection = Connection())
			{
				connection.Open();
			}
		}

		/// <summary>
		/// Perform some basic insertion tests using primary and foreign
		/// key constraints.
		/// </summary>
		[Test]
		[ReflectionPermission(SecurityAction.LinkDemand, Unrestricted=true)]
		public void RelationTest1()
		{
			Helper.Print("\n---\n--- Starting RelationTest1\n---\n");
			DataSet ds = base.MakeDataSet();
			base.DoRelationTest(ds, new JetDataSchemaAdapter());
		}

		/// <summary>
		/// Perform some basic insertion tests using primary and foreign
		/// key constraints.
		/// </summary>
		[Test]
		[ReflectionPermission(SecurityAction.LinkDemand, Unrestricted=true)]
		public void RelationTest2()
		{
			Helper.Print("\n---\n--- Starting RelationTest2\n---\n");

			string file = "RelationTestFromFile.xsd";

			using (DataSet ds = MakeDataSet())
			{
				ds.WriteXmlSchema(file);
				ds.Dispose();
			}

			using (DataSet ds = new DataSet())
			{
				ds.ReadXmlSchema(file);
				ds.DataSetName = this.Catalog;
				DoRelationTest(ds, new JetDataSchemaAdapter());
			}
		}

		/// <summary>
		/// Tests the creation (and subsequent removal) of a database.
		/// </summary>
		[Test]
		// AS: this test fails with the message 
			// "'abstract_element_definition__slot__members__abstract_element_definition' 
			// is not a valid name.  Make sure that it 
			// 'abstract_element_definition__slot__members__abstract_element_definition' is not a 
			// valid name.  Make sure that it does not include invalid characters or punctuation 
			// and that it is not too long.
			public void CreateFromOntology()
		{
			Helper.Print("\n---\n--- Starting CreateFromOntology\n---\n");

			DataSchemaAdapter creator = new JetDataSchemaAdapter();
			DataSet dataSet = new DataSet(this.Catalog);

			string xsdFile = ConfigurationManager.AppSettings["Test.ObjectModelXsd"];
			if (xsdFile == null || xsdFile.Length == 0)
				xsdFile = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Test\ObjectModel.xsd");

			dataSet.ReadXmlSchema(xsdFile);
			dataSet.DataSetName = this.Catalog;

			creator.Create(dataSet, true);

			Assertion.Assert(File.Exists(JetDataSchemaAdapter.GetPath(this.Catalog)));

			//
			// Can we open it?
			//
			using (IDbConnection connection = Connection())
			{
				connection.Open();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			CleanUp();
		}


		/// <summary>
		/// Cleanup code to ensure that the tests don't leave anything
		/// behind in the database.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			CleanUp();
		}

		/// <summary>
		/// 
		/// </summary>
		public void CleanUp()
		{
			File.Delete(JetDataSchemaAdapter.GetPath(Catalog));
		}
	}
}