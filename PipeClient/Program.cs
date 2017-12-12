using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client Running");
            ConversationWithTheServer();
        }

        private static void ReceiveByteAndRespond()
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("test-pipe"))
            {
                namedPipeClient.Connect();
                Console.WriteLine(namedPipeClient.ReadByte());
                namedPipeClient.WriteByte(2);

                Console.ReadKey();
            }
        }

        private static void ReceiveByteAndRespondContinuous()
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("test-pipe"))
            {
                namedPipeClient.Connect();
                Console.WriteLine("Client connected to the named pipe server. Waiting for server to send first byte...");
                Console.WriteLine("The server sent a single byte to the client: {0}", namedPipeClient.ReadByte());
                Console.Write("Provide a byte response from client: ");
                string b = Console.ReadLine();
                Console.WriteLine("About to send byte {0} to server.", b);
                namedPipeClient.WriteByte(Encoding.UTF8.GetBytes(b).First());
                while (b != "x")
                {
                    Console.WriteLine("The server sent a single byte to the client: {0}", namedPipeClient.ReadByte());
                    Console.Write("Provide a byte response from client: ");
                    b = Console.ReadLine();
                    Console.WriteLine("About to send byte {0} to server.", b);
                    namedPipeClient.WriteByte(Encoding.UTF8.GetBytes(b).First());
                }

                Console.WriteLine("Client chose to disconnect...");
            }
        }

        private static void SendSingleMessageToServer()
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("test-pipe"))
            {
                Console.Write("Enter a message to be sent to the server: ");
                string message = Console.ReadLine();
                namedPipeClient.Connect();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                namedPipeClient.Write(messageBytes, 0, messageBytes.Length);
            }
        }

        #region basic conversation with messages
        private static void ConversationWithTheServer()
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream(".", "test-pipe", PipeDirection.InOut))
            {
                namedPipeClient.Connect();
                Console.WriteLine("Client connected to the named pipe server. Waiting for server to send the first message...");

                namedPipeClient.ReadMode = PipeTransmissionMode.Message;
                string messageFromServer = ProcessSingleReceivedMessage(namedPipeClient);
                Console.WriteLine("The server is saying {0}", messageFromServer);

                Console.Write("Write a response: ");
                string response = Console.ReadLine();
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                namedPipeClient.Write(responseBytes, 0, responseBytes.Length);

                while (response != "x")
                {
                    messageFromServer = ProcessSingleReceivedMessage(namedPipeClient);
                    Console.WriteLine("The server is saying {0}", messageFromServer);
                    Console.Write("Write a response: ");
                    response = Console.ReadLine();
                    responseBytes = Encoding.UTF8.GetBytes(response);
                    namedPipeClient.Write(responseBytes, 0, responseBytes.Length);
                }
            }
        }

        private static string ProcessSingleReceivedMessage(NamedPipeClientStream namedPipeClient)
        {
            StringBuilder messageBuilder = new StringBuilder();
            string messageChunk = string.Empty;
            byte[] messageBuffer = new byte[5];
            do
            {
                namedPipeClient.Read(messageBuffer, 0, messageBuffer.Length);
                messageChunk = Encoding.UTF8.GetString(messageBuffer);
                messageBuilder.Append(messageChunk);
                messageBuffer = new byte[messageBuffer.Length];
            }
            while (!namedPipeClient.IsMessageComplete);
            return messageBuilder.ToString();
        }
        #endregion
    }
}
