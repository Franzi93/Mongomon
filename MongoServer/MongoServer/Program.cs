using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoServer
{


    class Program
    {
        public static MongoClient client;
       

        static void Main(string[] args)
        {
            var connectionString = "mongodb://localhost:27017";

            client = new MongoClient(connectionString);

            WebServer ws = new WebServer(SendResponse, "http://localhost:8080/game/");
            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();

            ws.Stop();
        }

        static async Task GetName()
        {
            IMongoDatabase db = client.GetDatabase("testdb");
            var collection = db.GetCollection<BsonDocument>("students");
            var filter = Builders<BsonDocument>.Filter.Eq("firstname", "Peter");
            var x = await collection.FindAsync(filter);
            x.First();
        }

        static async Task SetName()
        {
            IMongoDatabase db = client.GetDatabase("testdb");
            var collection = db.GetCollection<BsonDocument>("students");
            var document = new BsonDocument
            {
              {"firstname", BsonValue.Create("Peter")},
              {"lastname", new BsonString("Mbanugo")},
              { "subjects", new BsonArray(new[] {"English", "Mathematics", "Physics"}) },
              { "class", "JSS 3" },
              { "age", 45}
            };
            await collection.InsertOneAsync(document);
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            Console.WriteLine("A simple response.");
            GetName().Wait();

            //TODO following will be json
            return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }
    }






    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
