using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckListToolWPF
{
    public class Log
    {
        static string filepath = Properties.Settings.Default.LogPath;
        public Log()
        {}

        public static void CreateLogIfNotExist()
        {
            filepath +=  "\\" + Environment.UserName + ".txt";
        }
        public static void Write(string logMessage)
        {
            using (var w = new StreamWriter(filepath, true))
            {
                w.WriteLine("{0} {1}: {2}", DateTime.Now.ToShortDateString(),
                    DateTime.Now.ToShortTimeString(), logMessage);
            }
        }

    }
}
