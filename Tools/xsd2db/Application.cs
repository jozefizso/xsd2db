using System;
using System.Data;
using System.IO;
using System.Reflection;
using Xsd2Db.CommandLineParser;
using Xsd2Db.Data;

namespace Xsd2Db
{
	/// <summary>
	/// Summary description for Application.
	/// </summary>
	public class Application
	{
		protected readonly string[] args;
		public readonly CommandLine commandline;

		public const string Extension = ".xsd";

		public Application(string[] args)
		{
			this.args = args;
			Pattern.DefaultParserType = Pattern.ParserType.Advanced;
			Schema = new DataSet();
			this.commandline = new CommandLine();
			this.commandline.AutoValidate = false;
			this.commandline.AllowUnrecognized = false;
			this.commandline.Bind(this);
		}

		public void Initialize()
		{
			this.commandline.Resolve();
			if (!this.ShowHelp)
			{
				this.commandline.Validate();

				if ((this.Type == DatabaseType.Sql)
					&& !this.commandline.Parameters["l"].IsExplicitlySet)
				{
					throw new ArgumentException(
						"The SQL database type requires a location");
				}

				if (Path.GetExtension(this.SchemaFile).ToLower() != Extension)
				{
					this.SchemaFile = String.Concat(this.SchemaFile, Extension);
				}

				this.Schema.ReadXmlSchema(this.SchemaFile);

				if (this.commandline.Parameters["name"].IsExplicitlySet)
				{
					this.Schema.DataSetName = this.Name;
				}
			}
		}

		public void WriteHelp(TextWriter writer)
		{
			this.commandline.WriteHelp(writer);
		}

		/// <summary>
		/// 
		/// </summary>
		[Parameter("name", "n",
			"the name of the database to be created")] public string Name;

		/// <summary>
		/// This property stores the optional table prefix that 
		/// will be given to each table
		/// </summary>
		[Parameter("dbowner", "o",
			"This is a prefix added to each table",
			Default="dbo")] public string DbOwner;

		/// <summary>
		/// This property stores the optional table prefix that 
		/// will be given to each table
		/// </summary>
		[Parameter("tableprefix", "p",
			"This is a prefix added to each table",
			Default="")] public string TablePrefix;

        /// <summary>
		/// This property stores the optional table prefix that 
		/// will be given to each table
		/// </summary>
		[Parameter("existing", "e",
            "Existing Database",
            Default = false)]
        public bool Exisitng;

        /// <summary>
        /// This property stores the data set which contains the
        /// schema definition.
        /// </summary>
        public readonly DataSet Schema;

		/// <summary>
		/// The property denotes the source XSD file provided by the user.
		/// </summary>
		[Parameter("schema", "s",
			"The the XSD file containing the schema",
			IsRequired = true)] public string SchemaFile;

		public enum DatabaseType
		{
			Unspecified,
			Jet,
			Sql,
			Ole,
			OleDb = Ole
		}

		/// <summary>
		/// The type of database specified by the user.
		/// </summary>
		[Parameter("type", "t",
			"The type of database to connect to [Jet | Sql | OleDb]",
			Default=DatabaseType.Unspecified,
			IsRequired = true)] public DatabaseType Type;

		/// <summary>
		/// The property denotes the database host if SQL server is used.
		/// </summary>
		[Parameter("location", "l",
			"The type of database to connect to (servername)")] public string Location;

		/// <summary>
		/// This property indicates whether or not the user has requested
		/// that any existing database be overwritten from the schema.  This
		/// option should be used with extreme care.
		/// </summary>
		[Parameter("force", "f",
			"drop the database if it exists.",
			Default=false)] public bool Force;

		/// <summary>
		/// This property denotes whether or not the user has asked for the
		/// help/usage information to be printed.
		/// </summary>
		[Parameter("help", "h",
			"Display usage instructions",
			Default=false)] public bool ShowHelp;

		/// <summary>
		/// The object which is used to create the database
		/// </summary>
		public DataSchemaAdapter Creator
		{
			get
			{
				DataSchemaAdapter result = null;
				switch (this.Type)
				{
					case DatabaseType.Sql:
						result = new SqlDataSchemaAdapter(Location);
						break;
					case DatabaseType.Jet:
						result = new JetDataSchemaAdapter();
						break;
					case DatabaseType.OleDb:
					default:
						throw new ArgumentException(
							"The supplied database type is not supported",
							this.Type.ToString());
				}

				return result;
			}
		}

		/// <summary>
		/// Prints a copyright message to the console.
		/// </summary>
		private static void PrintCopyright()
		{
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
			Console.WriteLine(
				string.Format("xsd2db {0} - Utility to generate database based on a given XSD Schema", version));
            Console.WriteLine(
                    "More Information: http://xsd2db.sourceforge.net");
            Console.WriteLine(
                "Supported by: https://threenine.co.uk");
        }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application application = new Application(args);
			try
			{
				PrintCopyright();
			  
				if (!application.commandline.TextIsValid)
				{
					application.WriteHelp(Console.Out);
				}
				else
				{
					application.Initialize();
					if (application.ShowHelp)
					{
						application.WriteHelp(Console.Out);
					}
					else
					{
						application.Creator.Create(
							application.Schema,
							application.Force,
							application.TablePrefix,
							application.DbOwner,
                            application.Exisitng
							);
					}
				}
			}
			catch (ArgumentException e)
			{
				Console.WriteLine("-- Error --");
				Console.WriteLine(e.Message);
				Console.WriteLine("-- Instructions --");
				application.WriteHelp(Console.Out);
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine(
					"File not found: {0}",
					application.SchemaFile);
			}
			catch (DirectoryNotFoundException)
			{
				Console.WriteLine(
					"Path not valid: {0}",
					application.SchemaFile);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}