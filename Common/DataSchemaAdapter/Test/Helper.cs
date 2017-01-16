using System;

namespace Xsd2Db.Data.Test
{
	/// <summary>
	/// Summary description for Helper.
	/// </summary>
	internal sealed class Helper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public static void Print(string text)
		{
			Console.WriteLine(text.Replace("\n", Console.Out.NewLine));
		}
	}
}