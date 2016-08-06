using System.IO;
using System.Net.Sockets;

namespace Singular.Grid.Communication
{
    public static class CommunicationEndpointConverter<T>
    {
        public static CommunicationEndpoint<T> FromTcpClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            CommunicationEndpoint<T> endpoint = new CommunicationEndpoint<T>(new StreamReader(stream),
                new StreamWriter(stream), tcpClient.Close);
            return endpoint;
        }
    }
}