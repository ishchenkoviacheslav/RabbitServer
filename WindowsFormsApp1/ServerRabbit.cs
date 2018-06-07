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
        private List<ClientPeer> ClientsList = new List<ClientPeer>();
        public IConnection connection;
        IModel channel;
        //need do that. If server will be close, must kill that thread
        public CancellationTokenSource tokenSource = null;
        public ServerRabbit()
        {
            logger.Info("Server started");
            ConnectionFactory factory = new ConnectionFactory() { UserName = "slavik", Password = "slavik", HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            //channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "rpc_queue", autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                IBasicProperties props = ea.BasicProperties;
                channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: null, body: new byte[] { 1, 2, 3 });

                return;
                //IBasicProperties props = ea.BasicProperties;
                if (!ClientsList.Exists((c) => c.QeueuName == props.ReplyTo))
                {
                    ClientsList.Add(new ClientPeer() { QeueuName = props.ReplyTo, LastUpTime = DateTime.UtcNow });
                    logger.Info($"Was added a client: {props.ReplyTo}");
                }
                Object obtained = ea.Body.Deserializer();
                switch (obtained)
                {
                    //case ObjectCategory.Message:
                    //    Message mess = ea.Body.Deserializer<Message>();
                    //    IBasicProperties replyPropMessage = channel.CreateBasicProperties();
                    //    replyPropMessage.CorrelationId = props.CorrelationId;
                    //    foreach (ClientPeer p in ClientsList)
                    //    {
                    //        if (props.ReplyTo != p.qName)
                    //            channel.BasicPublish(exchange: "", routingKey: p.qName, basicProperties: replyPropMessage, body: mess.Serializer());
                    //    }

                    //    break;
                    case PingPeer p:
                        //ClientsList.ForEach((c) =>
                        //{
                        //    if (c.QeueuName == ea.BasicProperties.ReplyTo)
                        //    {
                        //        logger.Info($"client's queue: {c.QeueuName} refreshed");
                        //        c.LastUpTime = DateTime.UtcNow;
                        //    }
                        //});
                        ClientPeer peer = ClientsList.FirstOrDefault((pr) => pr.QeueuName == ea.BasicProperties.ReplyTo);
                        if(peer != null)
                        {
                            peer.LastUpTime = DateTime.UtcNow;
                        }
                        break;
                    //case ObjectCategory.GameData:
                    //    GameData game = ea.Body.Deserializer<GameData>();
                    //    IBasicProperties replyPropGame = channel.CreateBasicProperties();
                    //    replyPropGame.CorrelationId = props.CorrelationId;
                    //    foreach (ClientPeer p in ClientsList)
                    //    {
                    //        if (props.ReplyTo != p.qName)
                    //            channel.BasicPublish(exchange: "", routingKey: p.qName, basicProperties: replyPropGame, body: game.Serializer());
                    //    }
                    //    break;
                    default:
                        logger.Error("Type is different!");
                        break;
                }//switch
            };
            tokenSource = new CancellationTokenSource();
            PingToAll();
        }//ctor

        public void Dispose()
        {
            tokenSource.Cancel();
            channel.Close();
            connection.Close();
            logger.Info("Server stoped");
        }
        ~ServerRabbit()
        {
            tokenSource.Dispose();
            Dispose();
        }
        public void Cancel()
        {
            //think its dont need... 
            tokenSource.Cancel();
            logger.Info("Server was paused(ping stoped)");
        }
        public void PingToAll()
        {
            Task.Run(() =>
            {
                logger.Info("Ping process started...");
                while (true)
                {
                    //need do that. If server will be close, must kill that thread
                    if (tokenSource.Token.IsCancellationRequested)
                    {
                        logger.Info("Cancellation token work!!!!!!!!!!!!!!!!!!!!!!!");
                        tokenSource.Token.ThrowIfCancellationRequested();
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
                    foreach (ClientPeer c in ClientsList)
                    {
                        channel.BasicPublish(exchange: "", routingKey: c.QeueuName, basicProperties: null, body: new PingPeer().Serializer());
                    }

                    Thread.Sleep(1000);
                }
            }, tokenSource.Token);
        }
    }
}
