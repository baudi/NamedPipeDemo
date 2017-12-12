using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server Running");
            //ConversationWithTheClient();
            ReceiveObjectFromClient();
        }

        private static void SendByteAndReceiveResponse()
        {
            using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("test-pipe"))
            {
                namedPipeServer.WaitForConnection();
                namedPipeServer.WriteByte(1);
                int byteFromClient = namedPipeServer.ReadByte();
                Console.WriteLine(byteFromClient);
            }

            Console.ReadKey();
        }

        private static void SendByteAndReceiveResponseContinuous()
        {
            using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("test-pipe"))
            {
                Console.WriteLine("Server waiting for a connection...");
                namedPipeServer.WaitForConnection();

                Console.Write("A client has connected, send a byte from the server: ");
                string b = Console.ReadLine();
                Console.WriteLine("About to send byte {0} to client.", b);
                namedPipeServer.WriteByte(Encoding.UTF8.GetBytes(b).First());

                Console.WriteLine("Byte sent, waiting for response from client...");
                int byteFromClient = namedPipeServer.ReadByte();

                Console.WriteLine("Received byte response from client: {0}", byteFromClient);
                while (byteFromClient != 120)
                {
                    Console.Write("Send a byte response: ");
                    b = Console.ReadLine();
                    Console.WriteLine("About to send byte {0} to client.", b);
                    namedPipeServer.WriteByte(Encoding.UTF8.GetBytes(b).First());
                    Console.WriteLine("Byte sent, waiting for response from client...");
                    byteFromClient = namedPipeServer.ReadByte();
                    Console.WriteLine("Received byte response from client: {0}", byteFromClient);
                }
                Console.WriteLine("Server exiting, client sent an 'x'...");
            }
        }

        private static void ReceiveSingleMessageFromClient()
        {
            using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("test-pipe", PipeDirection.InOut,
                1, PipeTransmissionMode.Message))
            {
                namedPipeServer.WaitForConnection();
                StringBuilder messageBuilder = new StringBuilder();
                string messageChunk = string.Empty;
                byte[] messageBuffer = new byte[5];
                do
                {
                    namedPipeServer.Read(messageBuffer, 0, messageBuffer.Length);
                    messageChunk = Encoding.UTF8.GetString(messageBuffer);
                    messageBuilder.Append(messageChunk);
                    messageBuffer = new byte[messageBuffer.Length];
                }
                while (!namedPipeServer.IsMessageComplete);

                Console.WriteLine(messageBuilder.ToString());
            }
        }

        #region basic conversation with messages
        private static void ConversationWithTheClient()
        {
            using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("test-pipe", PipeDirection.InOut,
                1, PipeTransmissionMode.Message))
            {
                Console.WriteLine("Server waiting for a connection...");
                namedPipeServer.WaitForConnection();

                Console.Write("A client has connected, send a greeting from the server: ");
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                namedPipeServer.Write(messageBytes, 0, messageBytes.Length);

                string response = ProcessSingleReceivedMessage(namedPipeServer);
                Console.WriteLine("The client has responded: {0}", response);
                while (response != "x")
                {
                    Console.Write("Send a response from the server: ");
                    message = Console.ReadLine();
                    messageBytes = Encoding.UTF8.GetBytes(message);
                    namedPipeServer.Write(messageBytes, 0, messageBytes.Length);
                    response = ProcessSingleReceivedMessage(namedPipeServer);
                    Console.WriteLine("The client is saying {0}", response);
                }

                Console.WriteLine("The client has ended the conversation.");
            }
        }

        private static string ProcessSingleReceivedMessage(NamedPipeServerStream namedPipeServer)
        {
            StringBuilder messageBuilder = new StringBuilder();
            string messageChunk = string.Empty;
            byte[] messageBuffer = new byte[5];
            do
            {
                namedPipeServer.Read(messageBuffer, 0, messageBuffer.Length);
                messageChunk = Encoding.UTF8.GetString(messageBuffer);
                messageBuilder.Append(messageChunk);
                messageBuffer = new byte[messageBuffer.Length];
            }
            while (!namedPipeServer.IsMessageComplete);

            return messageBuilder.ToString();
        }
        #endregion

        private static void ReceiveObjectFromClient()
        {
            using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("orders", PipeDirection.InOut,
                1, PipeTransmissionMode.Message))
            {
                namedPipeServer.WaitForConnection();
                StringBuilder messageBuilder = new StringBuilder();
                string messageChunk = string.Empty;
                byte[] messageBuffer = new byte[5];
                do
                {
                    namedPipeServer.Read(messageBuffer, 0, messageBuffer.Length);
                    messageChunk = Encoding.UTF8.GetString(messageBuffer);
                    messageBuilder.Append(messageChunk);
                    messageBuffer = new byte[messageBuffer.Length];
                }
                while (!namedPipeServer.IsMessageComplete);
                Order order = JsonConvert.DeserializeObject<Order>(messageBuilder.ToString());
                Console.WriteLine("Customer {0} has ordered {1} {2} with delivery address {3}", order.CustomerName, order.Quantity, order.ProductName, order.Address);
            }
        }
    }
}
