using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CheckListToolWPF
{
    public class Log
    {
        static string filepath = Properties.Settings.Default.LogPath;
        private static Object locker = new Object();
        public Log()
        {}

        public static void CreateLogIfNotExist()
        {
            filepath +=  "\\" + Environment.UserName + ".txt";
        }
        public static void Write(string logMessage)
        {
            try
            {
                lock (locker)
                {
                    using (var w = new StreamWriter(filepath, true))
                    {
                        w.WriteLine("{0} {1}: {2}", DateTime.Now.ToShortDateString(),
                            DateTime.Now.ToShortTimeString(), logMessage);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Write("Error was catched in Log.CreateLogIfNotExist(): " + e);
                MessageBox.Show("Something went wrong, take a look at the log!");
                Application.Current.Shutdown();
            }
        }
    }
}
