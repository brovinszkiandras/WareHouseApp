using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WH_APP_GUI
{
    class Debug
    {
        public static void CreateLog()
        {
            File.Create("log.txt").Close();
            StreamWriter streamWriter = new StreamWriter("log.txt");
            streamWriter.WriteLine($"Logged Errors(since: {DateTime.Now.ToString("yyyy/MM/dd:HH-mm")}): ");
        }
        public static bool ThereIsExistingLog()
        {
            if (File.Exists("log.txt"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void WriteError(string error)
        {
            StreamWriter ir = new StreamWriter("log.txt", true);
            ir.Write($"\n[{DateTime.Now.ToString("yyyy/MM/dd:HH-mm")}] ERROR: {error}");
            ir.Close();
        }
        public static void WriteError(Exception error)
        {
            StreamWriter ir = new StreamWriter("log.txt", true);
            ir.Write($"\n[{DateTime.Now.ToString("yyyy/MM/dd:HH-mm")}] ERROR: {error.Message}");
            ir.Close();
        }
    }
}
