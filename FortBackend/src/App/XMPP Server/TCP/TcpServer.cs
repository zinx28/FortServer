using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using FortBackend.src.App.Utilities;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using FortBackend.src.App.XMPP_Server.Helpers.Globals.Data;
using System.Xml.Linq;
using System.Xml;

namespace FortBackend.src.App.XMPP_Server.TCP
{
    public class TcpServer
    {
        private TcpListener tcpListener;
        private CancellationTokenSource cancellationTokenSource;
        private X509Certificate2 certificate;
        private X509Certificate2 caCertificate;
        public TcpServer(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Start()
        {
            tcpListener.Start();
            var FindPfxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "Certificates", "FortBackend.pfx");
            if(!File.Exists(FindPfxPath))
            {
                Logger.Log($"Couldn't find FortBackend.pfx" , "TCP");
                tcpListener.Stop();
            }
            certificate = new X509Certificate2(FindPfxPath, "");
            Logger.Log($"TCP server started on port {((IPEndPoint)tcpListener.LocalEndpoint).Port}", "TCP");

            while (true)
            {
                
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    Console.WriteLine("NEW CONNECTION!!");
                    string clientId = Guid.NewGuid().ToString(); // GENERATES A NEW CLIENTID
                    _ = Task.Run(() => HandleTcpClientAsync(client, clientId));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        // this is the worst tcp server ever that will always need a recode and prob won't work till the end
        private async Task HandleTcpClientAsync(TcpClient client, string clientId)
        {
            DataSaved_TCP dataSaved = new DataSaved_TCP();
            try
            {
                DataSaved_TCP.connectedClients.TryAdd(clientId, client);
                
               
                //byte[] buffer = new byte[0];
                //XDocument xmlDoc;
                // NetworkStream stream = client.GetStream();
               // string logFilePath = "test.txt";

                //  using (StreamWriter sw = File.AppendText(logFilePath))

                //
                // Requests are normal until it hits starttls
                // you don't need to handle the open thing instead you handle sessions
                using (NetworkStream stream = client.GetStream())
                {
                    StringBuilder receivedMessageBuilder = new StringBuilder();
                    byte[] buffer = new byte[1024];
                    int totalBytesRead = 0;
                    bool tlsStarted = false;
                    XDocument xmlDoc = null;
                    StringBuilder receivedDataBuilder = new StringBuilder();

                    while (client.Connected)
                    {

                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                       
                        receivedDataBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Logger.Log($"Received message: {receivedData}");

                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            Async = true
                        };

                        using (StringReader stringReader = new StringReader(receivedData))
                        using (XmlReader reader = XmlReader.Create(stringReader, settings))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "stream:stream")
                                {
                                    Console.WriteLine("chat");

                                    break;
                                }
                            }
                        }

                               
                       
                       

                        //if(xmlDoc != null)
                        //{
                        //    switch (xmlDoc.Root?.Name.LocalName)
                        //    {
                        //        case "stream:stream":
                        //            Console.WriteLine("chat");
                        //            break;
                        //        default: break;
                        //    }
                        // }
                        // receivedMessage += chunk;
                    }

                }
                //using (SslStream sslStream = new SslStream(client.GetStream(), false))
                //{
                //    sslStream.AuthenticateAsServer(certificate, true, System.Security.Authentication.SslProtocols.Tls12, false);

                //    StreamReader reader = new StreamReader(sslStream);
                //    string message = reader.ReadLine();


                //    Console.WriteLine(message);
                //}

                    //using (StreamWriter sw = File.AppendText(logFilePath))
                    //using (NetworkStream stream = client.GetStream())
                    //{
                    //    StringBuilder receivedMessageBuilder = new StringBuilder();
                    //    byte[] buffer = new byte[1024];
                    //    int totalBytesRead = 0;
                    //    bool tlsStarted = false;



                    //    while (true)
                    //    {
                    //        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    //        if (bytesRead == 0)
                    //        {
                    //            break;
                    //        }

                    //        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    //        receivedMessageBuilder.Append(receivedData);
                    //        string receivedMessage = receivedMessageBuilder.ToString();
                    //        Console.WriteLine($"Received message: {receivedMessage}");
                    //       // if (receivedData.EndsWith(">"))
                    //       // {

                    //            sw.WriteLine($"Received message: {receivedMessage}");
                    //        Console.WriteLine("TEST");

                    //        if (receivedMessage.Contains("stream:stream"))
                    //           {
                    //                string responseXml1 = "<?xml version='1.0' encoding='UTF-8'?>" +
                    //                    "<stream:stream xmlns:stream='http://etherx.jabber.org/streams' " +
                    //                    $"xmlns='jabber:client' from='127.0.0.1'  id='{clientId}' " +
                    //                    "xml:lang='und' version='1.0'>";


                    //                byte[] responseBytes1 = Encoding.UTF8.GetBytes(responseXml1);
                    //                await stream.WriteAsync(responseBytes1, 0, responseBytes1.Length);
                    //                Console.WriteLine("Response sent");
                    //                await Task.Delay(100);
                    //                string responseXml2 = "<stream:features><starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'><required/></starttls>" +
                    //                    "<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'><mechanism>PLAIN</mechanism></mechanisms></stream:features>";


                    //                byte[] responseBytes2 = Encoding.UTF8.GetBytes(responseXml2);
                    //                await stream.WriteAsync(responseBytes2, 0, responseBytes2.Length);


                    //            }
                    //            if (!tlsStarted && receivedMessage.Contains("starttls"))
                    //            {
                    //                string proceedXml = "<proceed xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>";
                    //                byte[] proceedBytes = Encoding.UTF8.GetBytes(proceedXml);
                    //                await stream.WriteAsync(proceedBytes, 0, proceedBytes.Length);
                    //                using (SslStream sslStream = new SslStream(stream, false))
                    //                {
                    //                    await sslStream.AuthenticateAsServerAsync(certificate, false, System.Security.Authentication.SslProtocols.Tls11, true);
                    //                    tlsStarted = true;
                    //                }
                    //            }

                    //            receivedMessageBuilder.Clear();
                    //      //  }
                    //    }

                    //    if (tlsStarted)
                    //    {

                    //        using (SslStream sslStream = new SslStream(stream, false))
                    //        {
                    //            while (true)
                    //            {
                    //                int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);
                    //                if (bytesRead == 0)
                    //                {
                    //                    break;
                    //                }

                    //                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    //                string receivedMessage = receivedMessageBuilder.ToString();
                    //                receivedMessageBuilder.Append(receivedData);

                    //                Console.WriteLine($"Received message: {receivedMessage}");

                    //                receivedMessageBuilder.Clear();
                    //            }
                    //        }
                    //    }
                    //}
                    //using (SslStream sslStream = new SslStream(client.GetStream(), false))
                    //{
                    //    //SslStream sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate);
                    //    sslStream.AuthenticateAsServer(certificate, false, SslProtocols.None, false);

                    //    try
                    //    {
                    //        byte[] buffer = new byte[1024];
                    //        StringBuilder receivedMessageBuilder = new StringBuilder();

                    //        while (true)
                    //        {
                    //            int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);
                    //            if (bytesRead == 0)
                    //            {
                    //                break;
                    //            }

                    //            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    //            receivedMessageBuilder.Append(receivedData);

                    //            sw.WriteLine($"Received {bytesRead} bytes: {receivedData}");

                    //            if (receivedData.EndsWith(">"))
                    //            {
                    //                string receivedMessage = receivedMessageBuilder.ToString();
                    //                Console.WriteLine($"Received message: {receivedMessage}");

                    //                // Reset message builder for next message
                    //                receivedMessageBuilder.Clear();
                    //            }
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        sw.WriteLine($"Error reading data: {ex.Message}");
                    //        Console.WriteLine($"Error reading data: {ex.Message}");
                    //    }
                    //}

                    //using (NetworkStream stream = client.GetStream())
                    //{
                    //    byte[] buffer = new byte[1024];
                    //    int bytesRead;

                    //    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    //    {
                    //        buffer = new byte[1024];

                    //        //int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    //        //if (bytesRead == 0)
                    //        //{
                    //        //    // Console.WriteLine("Client disconnected!");
                    //        //    break;
                    //        //}
                    //        Console.WriteLine("Received BYTE: " + buffer);
                    //        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    //        receivedData.Append(data);


                    //        Console.WriteLine("Received TCP message: " + data);
                    //        if (data.EndsWith(">"))
                    //        {
                    //            Console.WriteLine("TRUEW");
                    //        }

                    //        if (data.EndsWith(">"))
                    //        {
                    //            string completeMessage = receivedData.ToString();
                    //            Console.WriteLine("Received TCP message: " + completeMessage);

                    //            JToken test = "";
                    //            try
                    //            {
                    //                test = JToken.Parse(receivedData.ToString());
                    //                Console.WriteLine("DISCONNETING USER AS FAKE RESPONSE // JSON");
                    //                //dataSaved.receivedMessage = "";
                    //                return;
                    //            }
                    //            catch (JsonReaderException ex)
                    //            {
                    //                //return; // wow
                    //            }
                    //            if (data.Contains("stream:stream"))
                    //            {
                    //                string responseXml1 = "<?xml version='1.0' encoding='UTF-8'?>" +
                    //        "<stream:stream xmlns:stream='http://etherx.jabber.org/streams' " +
                    //        $"xmlns='jabber:client' from='127.0.0.1'  id='{clientId}' " +
                    //        "xml:lang='und' version='1.0'>";


                    //                byte[] responseBytes1 = Encoding.UTF8.GetBytes(responseXml1);
                    //                await stream.WriteAsync(responseBytes1, 0, responseBytes1.Length);
                    //                Console.WriteLine("Response sent");
                    //                await Task.Delay(100);
                    //                string responseXml2 = "<stream:features><starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'><required/></starttls>" +
                    //                    "<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'><mechanism>PLAIN</mechanism></mechanisms></stream:features>";


                    //                byte[] responseBytes2 = Encoding.UTF8.GetBytes(responseXml2);
                    //                await stream.WriteAsync(responseBytes2, 0, responseBytes2.Length);
                    //                Console.WriteLine("Response sent");


                    //                //stream.Write(responseBytes, 0, responseBytes.Length);
                    //            }
                    //            if (!completeMessage.Contains("stream:stream"))
                    //            {
                    //                xmlDoc = XDocument.Parse(completeMessage);
                    //                Console.WriteLine($"Value of 'to' attribute: {xmlDoc.Root?.Name.LocalName}");
                    //                switch (xmlDoc.Root?.Name.LocalName)
                    //                {
                    //                    case "starttls":
                    //                        string proceedXml = "<proceed xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>";
                    //                        byte[] proceedBytes = Encoding.UTF8.GetBytes(proceedXml);
                    //                        await stream.WriteAsync(proceedBytes, 0, proceedBytes.Length);
                    //                        Console.WriteLine("Proceed sent");
                    //                        break;
                    //                    case "iq":
                    //                        Iq.Init(client, xmlDoc, clientId, dataSaved);
                    //                        break;
                    //                    default: break;
                    //                }

                    //                ClientFix.Init(client, dataSaved, clientId);

                    //            }

                    //            receivedData.Clear();
                    //        }
                    //        else
                    //        {
                    //            if (data.Contains("stream:stream"))
                    //            {
                    //                string responseXml1 = "<?xml version='1.0' encoding='UTF-8'?>" +
                    //            "<stream:stream xmlns:stream='http://etherx.jabber.org/streams' " +
                    //            $"xmlns='jabber:client' from='127.0.0.1'  id='{clientId}' " +
                    //            "xml:lang='und' version='1.0'>";


                    //                byte[] responseBytes1 = Encoding.UTF8.GetBytes(responseXml1);
                    //                await stream.WriteAsync(responseBytes1, 0, responseBytes1.Length);
                    //                Console.WriteLine("Response sent");
                    //                await Task.Delay(100);
                    //                string responseXml2 = "<stream:features><starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'><required/></starttls>" +
                    //                    "<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'><mechanism>PLAIN</mechanism></mechanisms></stream:features>";


                    //                byte[] responseBytes2 = Encoding.UTF8.GetBytes(responseXml2);
                    //                await stream.WriteAsync(responseBytes2, 0, responseBytes2.Length);
                    //                Console.WriteLine("Response sent");
                    //                //string startTlsXml = "<starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>";
                    //                //byte[] responseBytes = Encoding.UTF8.GetBytes(startTlsXml);
                    //                //Console.WriteLine("SENDING BACK");
                    //                //await client.Client.SendAsync(responseBytes, SocketFlags.None);
                    //                // client.Client.SendAsync
                    //                //string streamOpeningResponse = "<stream:stream to=\"127.0.0.1\" xmlns:stream=\"http://etherx.jabber.org/streams\" xmlns=\"jabber:client\" version=\"1.0\">";
                    //                //byte[] responseBytes = Encoding.UTF8.GetBytes(streamOpeningResponse);
                    //                // Console.WriteLine("SENDING BACK");
                    //                // await client.Client.SendAsync(responseBytes, SocketFlags.None);
                    //                receivedData.Clear();
                    //            }
                    //        }
                    //        Array.Clear(buffer, 0, buffer.Length);
                    //    }
                    //    Console.WriteLine("Client disconnected.");
                    //   // break;
                    //}



                    dataSaved.receivedMessage = "";


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }
    }
}
