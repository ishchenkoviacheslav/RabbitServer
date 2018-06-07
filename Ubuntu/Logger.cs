using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService
{
    class Logger
    {
        object obj = new object();
        public Logger()
        {
           
        }
        public void Info(string message)
        {
            RecordEntry("INFO", message);
        }
        public void Error(string message)
        {
            RecordEntry("ERROR", message);
        }
        private void RecordEntry(string fileEvent, string EventData)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(@"/home/" + "RabitMQMainClientlog.txt", true))
                {
                    writer.WriteLine(String.Format($"{DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss")} : [{fileEvent}] - {EventData}"));
                    writer.Flush();
                }
            }
        }
    }
}
