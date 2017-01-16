using System;
using System.IO;
using ADOX;

namespace Xsd2Db.Data
{
	/// <summary>
	/// Summary description for JetDataSchemaAdapter.
	/// </summary>
	public sealed class JetDataSchemaAdapter : AdoxDataSchemaAdapter
	{
		/// <summary>
		/// 
		/// </summary>
		internal const string Extension = ".mdb";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal static string GetPath(string name)
		{
			return Path.GetFullPath(
				Path.ChangeExtension(name, Extension));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal static string GetConnectionString(string name)
		{
			return String.Format(
				"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Engine Type=5;",
				GetPath(name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		protected override void DeleteCatalog(string name)
		{
			File.Delete(GetPath(name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		protected override void CreateCatalog(string name)
		{
			// Force the run-time to let go of the file!  Otherwise,
			// cleanup and other operations might fail because the file
			// will still be in use.

			Catalog catalog = new CatalogClass();
			catalog.Create(GetConnectionString(name));
			catalog.ActiveConnection = null;
			catalog = null;
			GC.Collect();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected override Catalog OpenCatalog(string name)
		{
			Catalog catalog = new CatalogClass();
			catalog.let_ActiveConnection(GetConnectionString(name));
			return catalog;
		}
	}
}