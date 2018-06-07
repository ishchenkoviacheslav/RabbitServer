using RabbitMQService;
using System;
using System.Threading;

namespace Ubuntu
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerRabbit rabbitServer = null;
            using (rabbitServer = new ServerRabbit())
            {
                while (true)
                {
                    Thread.Sleep(10000);
                    Console.WriteLine("service main client rabbitmq working...");
                }
            }
        }
    }
}
