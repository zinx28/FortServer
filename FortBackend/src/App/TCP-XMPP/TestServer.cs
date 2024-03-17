using FortBackend.src.App.TCP_XMPP.Helpers.Resources;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.Net.Mail;
using FortBackend.src.App.TCP_XMPP.Root;
using FortBackend.src.App.TCP_XMPP.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities;

namespace FortBackend.src.App.XMPP
{
    public class TcpServer
    {
        private TcpListener tcpListener;
        private CancellationTokenSource cancellationTokenSource;

        public TcpServer(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Start()
        {
            tcpListener.Start();
            Logger.Log($"TCP server started on port {Saved.DeserializeConfig.TCPXmppPort}", "TCP");

            while (true)
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    TcpClient client = await tcpListener.AcceptTcpClientAsync();
                    try
                    {
                        Console.WriteLine("NEW CONNECTION!!");
                        string clientId = Guid.NewGuid().ToString(); // GENERATES A NEW CLIENTID

                        await HandleTcpClientAsync(client, clientId);

                        //Task.Run(() => HandleClientAsync(client));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private async Task HandleTcpClientAsync(TcpClient client, string clientId)
        {
            DataSaved dataSaved = new DataSaved();
            try
            {
                DataSaved.connectedClients.TryAdd(clientId, client);
                byte[] buffer = new byte[0];
                XDocument xmlDoc;
                NetworkStream stream = client.GetStream();

                buffer = new byte[1024];
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    StringBuilder receivedData = new StringBuilder();
                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                           // Console.WriteLine("Client disconnected!");
                            break;
                        }

                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        receivedData.Append(data);


                        Console.WriteLine("Received TCP message: " + data);
                        if (data.EndsWith(">"))
                        {
                            Console.WriteLine("TRUEW");
                        }

                        if (data.EndsWith(">")) 
                        {
                            string completeMessage = receivedData.ToString();
                            Console.WriteLine("Received TCP message: " + completeMessage);

                            JToken test = "";
                            try
                            {
                                test = JToken.Parse(receivedData.ToString());
                                Console.WriteLine("DISCONNETING USER AS FAKE RESPONSE // JSON");
                                //dataSaved.receivedMessage = "";
                                return;
                            }
                            catch (JsonReaderException ex)
                            {
                                //return; // wow
                            }
                            if (data.Contains("stream:stream"))
                            {
                                string streamOpeningResponse = "<stream:stream to=\"127.0.0.1\" xmlns:stream=\"http://etherx.jabber.org/streams\" xmlns=\"jabber:client\" version=\"1.0\">";


                                byte[] responseBytes = Encoding.UTF8.GetBytes(streamOpeningResponse);


                                stream.Write(responseBytes, 0, responseBytes.Length);
                            }
                            if (!completeMessage.Contains("stream:stream"))
                            {
                                xmlDoc = XDocument.Parse(completeMessage);
                                Console.WriteLine($"Value of 'to' attribute: {xmlDoc.Root?.Name.LocalName}");
                                switch (xmlDoc.Root?.Name.LocalName)
                                {

                                    case "iq":
                                        Iq.Init(client, xmlDoc, clientId, dataSaved);
                                        break;
                                    default: break;
                                }

                                ClientFix.Init(client, dataSaved, clientId);

                            }
                            receivedData.Clear();
                        }else
                        {
                            if (data.Contains("stream:stream"))
                            {
                               // client.Client.SendAsync
                                string streamOpeningResponse = "<stream:stream to=\"127.0.0.1\" xmlns:stream=\"http://etherx.jabber.org/streams\" xmlns=\"jabber:client\" version=\"1.0\">";
                                byte[] responseBytes = Encoding.UTF8.GetBytes(streamOpeningResponse);
                                Console.WriteLine("SENDING BACK");
                                await client.Client.SendAsync(responseBytes, SocketFlags.None);
                            }
                        }
                    }
                    Console.WriteLine("Client disconnected.");
                    break;
                }


                
                dataSaved.receivedMessage = "";
                    
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
