using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Singular.Grid.Core.Exceptions;

namespace Singular.Grid.Communication
{
    public class CommunicationEndpoint<T>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Action CloseConnection;

        public CommunicationEndpoint(StreamReader reader, StreamWriter writer, Action closeConnection)
        {
            writer.AutoFlush = true;
            Reader = reader;
            Writer = writer;

            Converter = new JsonSerializer();
            MessageReceived += message => { logger.Info($"CommunicationnEndpoint received message {message}"); };
            CloseConnection = closeConnection;
        }

        private StreamReader Reader { get; }
        private StreamWriter Writer { get; }
        private JsonSerializer Converter { get; }

        private bool Executing { get; set; } = true;

        public event Action<T> MessageReceived = delegate { };
        public event Action EndpointClosed = delegate { };

        public async void Send(T message)
        {
            using (StringWriter textWriter = new StringWriter())
            {
                logger.Info($"CommunicationnEndpoint sending {message}");
                Converter.Serialize(textWriter, message);
                await Writer.WriteLineAsync(textWriter.ToString());
            }
        }

        public void Close()
        {
            Executing = false;
        }

        public async Task Process()
        {
            if (!MessageReceived.GetInvocationList().Any())
                throw new InvalidModelException(
                    $"There must be at least one handler for event {nameof(MessageReceived)}");

            Executing = true;

            while (Executing)
            {
                string content;

                try
                {
                    content = await Reader.ReadLineAsync();
                }
                catch (ObjectDisposedException)
                {
                    if (!Executing)
                        throw;

                    logger.Info("InvalidModelException thrown from reading the closed stream");
                    break;
                }

                if (content == null)
                {
                    logger.Info("The other end has closed the stream");
                    break;
                }

                T result;

                using (TextReader textReader = new StringReader(content))
                {
                    using (JsonReader jsonReader = new JsonTextReader(textReader))
                    {
                        result = Converter.Deserialize<T>(jsonReader);
                    }
                }

                MessageReceived(result);
            }

            CloseConnection();
            EndpointClosed();

            logger.Info("CommunicationnEndpoint stopping processing ");
        }
    }
}