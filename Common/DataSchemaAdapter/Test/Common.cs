using System;
using System.Data;
using NUnit.Framework;

namespace Xsd2Db.Data.Test
{
	/// <summary>
	/// Summary description for BaseTest.
	/// </summary>
	public abstract class Common
	{
		/// <summary>
		/// This should return a database connection to the catalog which has
		/// just been created (or it should fail by throwing an exception).
		/// </summary>
		/// <returns>A new data base connection on which to perform the test</returns>
		public abstract IDbConnection Connection();

		/// <summary>
		/// 
		/// </summary>
		public readonly string Catalog;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="catalog"></param>
		public Common(string catalog)
		{
			this.Catalog = catalog;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="commandText"></param>
		public abstract void ValidateExceptionOnDuplicateInsert(
			Exception exception,
			string commandText);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="commandText"></param>
		public abstract void ValidateExceptionOnBadForeignKeyInsert(
			Exception exception,
			string commandText);

		/// <summary>
		/// Creates a data set to be used with the DoRelationTest helper
		/// method.
		/// </summary>
		public DataSet MakeDataSet()
		{
			DataSet ds = new DataSet(this.Catalog);
			DataTable master = ds.Tables.Add("Master");
			DataColumn mid = master.Columns.Add("MasterID", Type.GetType("System.Guid"));
			DataColumn[] keys = new DataColumn[1];
			keys[0] = mid;
			master.PrimaryKey = keys;
			DataColumn uniqueCol = master.Columns.Add("UniqueText", Type.GetType("System.String"));
			uniqueCol.Unique = true; // TODO: Nobody cares about this yet!
			uniqueCol.AllowDBNull = true;
			uniqueCol.MaxLength = 80;

			DataTable detail = ds.Tables.Add("Detail");
			DataColumn did = detail.Columns.Add("DetailID", Type.GetType("System.Guid"));
			DataColumn mid_fk = detail.Columns.Add("MasterID_FK", Type.GetType("System.Guid"));
			DataColumn[] keys2 = new DataColumn[1];
			keys2[0] = did;
			detail.PrimaryKey = keys2;
			ds.Relations.Add(mid, mid_fk);

			return (ds);
		}

		/// <summary>
		/// Perform some basic insertion tests using primary and foreign
		/// key constraints.
		/// </summary>
		/// <param name="ds">the dataset from which to take the schema.
		/// This DataSet should be derived from one created by the
		/// <see cref="MakeDataSet"/> method.
		/// </param>
		/// <param name="creator">The DataSchemaAdapter to use for this
		/// test.</param>
		public void DoRelationTest(DataSet ds, DataSchemaAdapter creator)
		{
			//
			// Create a new database
			//

			creator.Create(ds, false);

			//
			// Do some work on the database
			//
			using (IDbConnection connection = this.Connection())
			{
				connection.Open();

				// Create a new record
				Guid masterId = Guid.NewGuid();
				string masterInsert = String.Format(
					"INSERT INTO Master (MasterID) VALUES ('{1}')",
					ds.DataSetName,
					masterId.ToString());
				Helper.Print(masterInsert);
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = masterInsert;
					Assertion.Assert(
						String.Format(
							"Insert query failed.\r\n\t{0}",
							command.CommandText),
						command.ExecuteNonQuery() == 1);
				}

				// Make sure that the same value cannot be inserted
				// a second time.
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = masterInsert;
					try
					{
						command.ExecuteNonQuery();
						Assertion.Assert(
							String.Format(
								"An exception should be thrown on duplicate insert!\r\n\t{0}",
								command.CommandText),
							false);
					}
					catch (Exception exception)
					{
						ValidateExceptionOnDuplicateInsert(
							exception,
							command.CommandText);
					}
				}

				// Add a valid foreign key reference
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = String.Format(
						"INSERT INTO Detail (DetailID, MasterID_FK) VALUES ('{1}', '{2}')",
						ds.DataSetName,
						Guid.NewGuid().ToString(),
						masterId.ToString());
					Helper.Print(command.CommandText);
					Assertion.Assert(
						String.Format(
							"Insert query failed.\r\n\t{0}",
							command.CommandText),
						command.ExecuteNonQuery() == 1);
				}

				// Add an invalid foreign key reference.
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = String.Format(
						"INSERT INTO Detail (DetailID, MasterID_FK) VALUES ('{1}', '{2}')",
						ds.DataSetName,
						Guid.NewGuid().ToString(),
						Guid.NewGuid().ToString());
					Helper.Print(command.CommandText);
					try
					{
						command.ExecuteNonQuery();
						Assertion.Assert(
							String.Format(
								"An exception should be thrown on duplicate insert!\r\n\t{0}",
								command.CommandText),
							false);
					}
					catch (Exception exception)
					{
						ValidateExceptionOnBadForeignKeyInsert(
							exception,
							command.CommandText);
					}
				}
			}
		}
	}

}