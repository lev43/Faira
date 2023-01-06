using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace Main {
    class Server
    {
        private static bool runServer = false;
        public static bool RunServer {get{return runServer;}}
        public static HttpListener listener = new HttpListener();
        public static string url = "http://localhost:8000/";
        public static int requestCount = 0;
        public static string PageData = "<html><p>404</p></html>";


        public static async Task Run()
        {
            Logger.Log("Server Is Running");
            PageData = await (new StreamReader(Program.RootPath+$"/Data/HTML/Base.html")).ReadToEndAsync();
            runServer = true;

            // Создаем считыватель входящих запросов.
            listener.Prefixes.Add(url);
            listener.Start();
            Logger.Log($"Listening for connections on {url}");

            // Запускаем сбор всех входящих запросов
            while (runServer)
            {
                // Ожидаем контекст запроса.
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Получаем Запрос и Ответ. (Request, Responce)
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Отправляем в логи информацию о запросе.
                Logger.Log($"Request #: {++requestCount}", "Info");
                Logger.Log(req.Url.ToString(), "Info");
                Logger.Log(req.HttpMethod, "Info");
                Logger.Log(req.UserHostName, "Info");
                Logger.Log(req.UserAgent, "Info");

                // Write the response info
                byte[] data = Encoding.UTF8.GetBytes(PageData);
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }
        public static void Stop() {
            runServer = false;

            listener.Close();
            Logger.Log("Server close");
        }
    }
}