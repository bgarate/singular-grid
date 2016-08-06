using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NLog;
using Singular.Grid.Core.Exceptions;

namespace Singular.Grid.Communication
{
    public class TcpServer
    {
        public delegate void ConnectionAceptedDelegate(TcpClient tcpClient);

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public TcpServer()
        {
            string ownHost = Dns.GetHostName();
            IPHostEntry hostInfo = Dns.GetHostEntry(ownHost);

            Address = hostInfo.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
        }

        public IPAddress Address { get; }
        public int Port { get; } = 35365;

        private TcpListener Listener { get; set; }

        private bool Executing { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        public event ConnectionAceptedDelegate ConnectionAcepted = delegate { };

        public void StopListening()
        {
            Executing = false;
            Listener.Stop();
        }

        public async void StartListening()
        {
            if (Executing)
                throw new InvalidModelException("Listener already started");

            Listener = new TcpListener(Address, Port);

            Listener.Start();
            logger.Info($"TcpServer listening on ({Address},{Port})");

            Executing = true;

            while (Executing)
            {
                TcpClient tcpClient = await Listener.AcceptTcpClientAsync();

                logger.Info(
                    $"TcpServer acepted a client from ({((IPEndPoint) tcpClient.Client.RemoteEndPoint).Address},{((IPEndPoint) tcpClient.Client.RemoteEndPoint).Port})");

                ConnectionAcepted(tcpClient);
            }

            logger.Info("TcpServer stopped listening");
        }
    }
}