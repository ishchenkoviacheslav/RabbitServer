using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService
{
    public partial class Service1 : ServiceBase
    {
        ServerRabbit serverRabbit = null;
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            //this.CanPauseAndContinue = true;
            this.AutoLog = true;
            serverRabbit = new ServerRabbit();
        }

        protected override void OnStart(string[] args)
        {
            if (!serverRabbit.connection.IsOpen)
            {
                serverRabbit = new ServerRabbit();
            }
        }

        protected override void OnStop()
        {
            serverRabbit.Dispose();
            base.OnStop();
        }
        //protected override void OnPause()
        //{
        //    serverRabbit.Cancel();
        //    base.OnPause();
        //}
        //protected override void OnContinue()
        //{
        //    if (!serverRabbit.connection.IsOpen)
        //    {
        //        serverRabbit = new ServerRabbit();
        //    }
        //    base.OnContinue();
        //}
    }
}
