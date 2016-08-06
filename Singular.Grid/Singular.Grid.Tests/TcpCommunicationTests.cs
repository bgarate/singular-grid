using System.Net.Sockets;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using Singular.Grid.Communication;

namespace Singular.Grid.Tests
{
    [TestFixture]
    public class TcpCommunicationTests
    {
        [SetUp]
        public void SetUp()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            ConsoleTarget target = new ConsoleTarget();
            config.AddTarget("Console", target);
            config.AddRuleForAllLevels(target);

            LogManager.Configuration = config;
        }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void OneClientOneServer()
        {
            TcpServer tcpServer = new TcpServer();

            tcpServer.ConnectionAcepted += c =>
            {
                CommunicationEndpoint<string> endpoint = CommunicationEndpointConverter<string>.FromTcpClient(c);

                endpoint.MessageReceived += message =>
                {
                    endpoint.Send($"Answering to '{message}' from client");
                    logger.Info(message);
                };
                endpoint.Process();
                tcpServer.StopListening();
            };

            tcpServer.StartListening();

            TcpClient client = new TcpClient(tcpServer.Address.ToString(), tcpServer.Port);
            CommunicationEndpoint<string> clientEndpoint = CommunicationEndpointConverter<string>.FromTcpClient(client);

            clientEndpoint.MessageReceived += s =>
            {
                logger.Info(s);
                clientEndpoint.Close();
            };

            Task a = clientEndpoint.Process();
            clientEndpoint.Send("Hola!");

            Task.WaitAll(a);
        }
    }
}