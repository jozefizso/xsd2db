using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallImport
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] fileEntries = Directory.GetFiles(@"C:\DLR7000\SystemConfig\DataSchemas", "*.xsd");
            foreach (string fileName in fileEntries)
            {


                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = @"C:\Users\gary\Downloads\XSD2DB-0.2-src\xsd2db\bin\Release\xsd2db.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.Arguments = "-f -l GaryVM -n doosh -s" + fileName + " -t sql -e";


                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using-statement will close.
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch(Exception ex)
                {
                    string sex = ex.Message;
                }

            }
        }
    }
}
