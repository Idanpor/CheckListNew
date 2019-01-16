using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CheckListToolWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, Microsoft.Shell.ISingleInstanceApp
    {
        private const string Unique = "UniqueCheckListTool";

    [STAThread]
        //public static void Main()
        //{
        //    if (Microsoft.Shell.SingleInstance<App>.InitializeAsFirstInstance(Unique))
        //    {
        //        var application = new MainWindow();

        //        //application.InitializeComponent();
        //        //application.Run();

        //        // Allow single instance code to perform cleanup operations
        //        Microsoft.Shell.SingleInstance<App>.Cleanup();
        //    }
        //}

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            // …

            return true;
        }
    }
}

       #endregion