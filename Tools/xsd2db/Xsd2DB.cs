using System;
using System.IO;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using Zks.Data;



namespace Zks.Data.Utilities
{
	internal class Xsd2DB
	{
		/// <summary>
		/// The main entry point for the Xsd2DB application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			PrintCopyright();

			if( args.Length == 0 )
			{
				PrintHelp();
				return;
			}

			bool bHelp = ParseHelp(args);
			if( bHelp )
			{
				PrintHelp();
				return;
			}

			bool bDropDB = ParseDropDB(args);
			string xsdFile = ParseXsd(args);

			try
			{
				int type = ParseType(args);
				string connectionString = ParseConnectionString(args);

				switch(type)
				{
					case 1:
						if( bDropDB )
						{
							try
							{
								Console.Write("Dropping database ");
								SqlConnection conn = new SqlConnection( connectionString );
								conn.Open();

								DataSet ds = new DataSet();
								ds.ReadXmlSchema( xsdFile );

								Console.Write(ds.DataSetName + "...");
								new SqlCommand("DROP DATABASE " + ds.DataSetName, conn).ExecuteNonQuery();
								conn.Close();
								Console.WriteLine("done!");
							}
							catch( SqlException ex )
							{
								Console.WriteLine( ex.Message );
								Console.WriteLine( "Continuing..." );
							}
						}

						Console.Write("Creating new database...");
						SqlDataSchemaAdapter a = new SqlDataSchemaAdapter( connectionString );
						a.CreateDatabase( xsdFile );

						Console.WriteLine("done!");
						break;
					default:
						Console.WriteLine("Database type is not specified or not supported", type );
						return;
				}
			}
			catch( FileNotFoundException ex )
			{
				/// Schema file not found
				Console.WriteLine( ex.Message );
				return;
			}
			catch( System.Data.SqlClient.SqlException ex )
			{
				/// Database already exists
				Console.WriteLine( ex.Message );
				return;
			}

		}

		static string ParseXsd( string[] args )
		{
			return args[0];
		}

		static bool ParseDropDB( string[] args )
		{
			foreach( string s in args )
			{
				if( s.StartsWith("/dropdatabase") )
					return true;
			}

			return false;
		}

		static bool ParseHelp( string[] args )
		{
			foreach( string s in args )
			{
				if( s.StartsWith("/h") || s.StartsWith("/help") )
					return true;
			}

			return false;
		}

		static int ParseType( string[] args )
		{
			string type = "";
			foreach( string s in args )
			{
				if( s.StartsWith("/dt:") )
				{
					type = s.Substring("/dt:".Length);
				}
				if( s.StartsWith("/databasetype:") )
				{
					type = s.Substring("/databasetype:".Length);
				}
			}

			if( String.Compare(type, "sql", true ) == 0 )
			{
				return 1;
			}

			return -1;
		}

		static string ParseConnectionString( string[] args )
		{
			string conn = "";
			foreach( string s in args )
			{
				if( s.StartsWith("/c:") )
				{
					conn = s.Substring("/c:".Length);
				}
				if( s.StartsWith("/connectionstring:") )
				{
					conn = s.Substring("/connectionstring:".Length);
				}
			}
			return conn;
		}

		static void PrintCopyright()
		{
			Console.WriteLine("Xsd2DB - Utility to generate database based on a given XSD Schema");
			Console.WriteLine("Copyright 2003 - Zero-Knowledge Systems Inc.");
		}

		static void PrintHelp()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("xsd2db.exe <schema>.xsd /connectionstring:<database server connection string>");
			Console.WriteLine("\n\n/connectionstring:\nConnection string to a database server. The string should not include an database name. Short Form is /c:");
			Console.WriteLine("/databasetype:\nType of the database. Currently supported type is 'Sql'. Short Form is /dt:");
			Console.WriteLine("/dropdatabase\nThe existing database will be dropped in case the DB server already contains the database with the same name. USE THIS OPTION WITH CARE!");
			Console.WriteLine("/help\nPrint help. Short Form is /h");
			Console.WriteLine("\n\nExample:\n");
			Console.WriteLine("xsd2db.exe MyDataSchema.xsd /dt:Sql /dropdatabase /connectionstring:\"Data Source=MyDBServer;Integrated Security=SSPI;Pooling='false'\"");
		}
	}
}
