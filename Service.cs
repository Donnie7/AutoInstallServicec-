using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.IO;

namespace sv
{
    class Program : ServiceBase
    {
        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;


            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new Program());
            }



        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.AppendAllText(@"C:\Temp\error.txt", ((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).InnerException.Message);
        }

        #region service code
        public Program()
        {
            this.ServiceName = "My Service";
            File.AppendAllText(@"C:\Temp\sss.txt", "aaa");

        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            File.AppendAllText(@"C:\Temp\sss.txt", "bbb");
        }

        protected override void OnStop()
        {
            base.OnStop();

            File.AppendAllText(@"C:\Temp\sss.txt", "ccc");
        }
        #endregion
    }

    [RunInstaller(true)]
    public class MyWindowsServiceInstaller : Installer
    {
        public MyWindowsServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "My Service";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "My Service";
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}