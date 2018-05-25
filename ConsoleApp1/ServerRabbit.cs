using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedClasses.DTO;
using SharedClasses.Serializator;

namespace RabbitMQService
{
    class ServerRabbit : IDisposable
    {
        Logger logger = new Logger();
        private List<ClientPropertise> ClientsList = new List<ClientPropertise>();
        IConnection connection;
        IModel channel;
        CancellationTokenSource tokenSource = null;
        public ServerRabbit()
        {
            logger.Info("Server will be started");
            ConnectionFactory factory = new ConnectionFactory() { UserName = "slavik", Password = "slavik", HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "rpc_queue", autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                IBasicProperties props = ea.BasicProperties;
                if (!ClientsList.Exists((c) => c.QeueuName == props.ReplyTo))
                {
                    ClientsList.Add(new ClientPropertise() { QeueuName = props.ReplyTo, LastUpTime = DateTime.UtcNow });
                    logger.Info("Was added a client");
                }
                Object obtained = ea.Body.Deserializer();
                switch (obtained)
                {
                    //case ObjectCategory.Message:
                    //    Message mess = ea.Body.Deserializer<Message>();
                    //    IBasicProperties replyPropMessage = channel.CreateBasicProperties();
                    //    replyPropMessage.CorrelationId = props.CorrelationId;
                    //    foreach (ClientPropertise p in ClientsList)
                    //    {
                    //        if (props.ReplyTo != p.qName)
                    //            channel.BasicPublish(exchange: "", routingKey: p.qName, basicProperties: replyPropMessage, body: mess.Serializer());
                    //    }

                    //    break;
                    case PingPeer p:
                        ClientsList.ForEach((c) =>
                        {
                            if (c.QeueuName == ea.BasicProperties.ReplyTo)
                            {
                                c.LastUpTime = DateTime.UtcNow;
                            }
                        });

                        return;
                        break;
                    //case ObjectCategory.GameData:
                    //    GameData game = ea.Body.Deserializer<GameData>();
                    //    IBasicProperties replyPropGame = channel.CreateBasicProperties();
                    //    replyPropGame.CorrelationId = props.CorrelationId;
                    //    foreach (ClientPropertise p in ClientsList)
                    //    {
                    //        if (props.ReplyTo != p.qName)
                    //            channel.BasicPublish(exchange: "", routingKey: p.qName, basicProperties: replyPropGame, body: game.Serializer());
                    //    }
                    //    break;
                    default:
                        logger.Error("Type is different!");
                        break;
                }
            };
            tokenSource = new CancellationTokenSource();
            PingToAll();
        }

        public void Dispose()
        {
            tokenSource.Dispose();
            channel.Close();
            logger.Info("Server will be stoped");
        }
        
        public void Cancel()
        {
            tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            logger.Info("Server was paused(ping stoped)");
        }
        public void PingToAll()
        {
            Task.Run(() =>
            {
                logger.Info("Ping process started...");
                while (true)
                {
                    if (tokenSource.Token.IsCancellationRequested)
                        tokenSource.Token.ThrowIfCancellationRequested();
                    foreach (ClientPropertise c in ClientsList)
                    {
                        channel.BasicPublish(exchange: "", routingKey: c.QeueuName, basicProperties: null, body: null);
                    }
                    //auto delete client after 5 second
                    ClientsList.RemoveAll((c) =>
                    {

                        if (DateTime.UtcNow.Subtract(c.LastUpTime) > new TimeSpan(0, 0, 5))
                        {
                            logger.Info("Client was deleted");
                            return true;
                        }
                        return false;
                    });

                    Thread.Sleep(1000);
                }
            }, tokenSource.Token);
        }
    }
}
