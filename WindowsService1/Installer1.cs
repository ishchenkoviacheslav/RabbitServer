using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace RabbitMQService
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public Installer1()
        {
            InitializeComponent();

            serviceInstaller = new ServiceInstaller()
            {
                StartType = ServiceStartMode.Manual,
                ServiceName = "1_RabbitMQServerTest"
            };
            processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
