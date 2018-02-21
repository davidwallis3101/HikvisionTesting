using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace HikvisionTesting
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        public static string BaseUrl => "http://192.168.0.152:80/";

        static void Main(string[] args)
        {
            //ProcessRepositories().Wait();

            Testing();
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        
        static void Testing()
        {
            var messageBuffer = string.Empty;

            var request = WebRequest.Create("http://192.168.0.152:80/ISAPI/Event/notification/alertStream");
            request.Credentials = new NetworkCredential("admin", "MyPassword");
            request.BeginGetResponse(ar =>
            {
                var req = (WebRequest)ar.AsyncState;
                // TODO: Add exception handling: EndGetResponse could throw
                using (var response = req.EndGetResponse(ar))
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    // This loop goes as long as the api is streaming
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        
                        if (line == "</EventNotificationAlert>")
                        {
                            messageBuffer += line;
                            GotMessage(messageBuffer);
                            messageBuffer = string.Empty;
                        }
                        else if (line.StartsWith("<"))
                        {
                            messageBuffer += line;
                        }
                    }
                }
            }, request);

            // Press Enter to stop program
            Console.ReadLine();
        }

        private static void GotMessage(string msg)
        {

            var mySerializer = new XmlSerializer(typeof(EventNotificationAlert));
            var stringReader = new StringReader(msg);

            var eventAlert = (EventNotificationAlert)mySerializer.Deserialize(stringReader);
            if (eventAlert.eventType == EventType.videoloss && eventAlert.channelID == 0) { return; } // Ingore video loss on channel 0

            Console.WriteLine($"DateTime: {eventAlert.dateTime} Channel: {eventAlert.channelID} Type: {eventAlert.eventType} Description: {eventAlert.eventDescription}");
        }

        public static HttpClient NewClient()
        {
            var client = new HttpClient();

            client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.AcceptEncoding.Clear();

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");

            var byteArray = Encoding.ASCII.GetBytes("admin:P@55w0rd");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return client;
        }
    }
}

// https://github.com/App-vNext/Polly/issues/272
// https://pastebin.com/mqsegRyd