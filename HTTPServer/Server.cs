using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        int Port;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);            //TODO: initialize this.serverSocket
            this.Port = portNumber;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint HostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(HostEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.            
            Console.WriteLine("Server is Listening.....");
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket client1 = serverSocket.Accept();
                Console.WriteLine("Connected...");
                Console.WriteLine("New client accepted {0}", client1.RemoteEndPoint);

                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(client1);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket newClient = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            newClient.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] recievedData = new byte[65536];
                     int msg_length = newClient.Receive(recievedData);
                    string data = Encoding.ASCII.GetString(recievedData);
                    Console.WriteLine(data);

                    // TODO: break the while loop if receivedLen==0
                    if (msg_length == 0)
                    {
                        Console.WriteLine("Connection Ended!");
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    Request clientrequest = new Request(data);


                    // TODO: Call HandleRequest Method that returns the response
                    Response serverresponse = HandleRequest(clientrequest);
                    string res = serverresponse.ResponseString;
                    Console.WriteLine(res);

                    byte[] response = Encoding.ASCII.GetBytes(res);


                    // TODO: Send Response back to client
                    newClient.Send(response);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            newClient.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content = "";
            string code = "";
            Response response;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    code = "400";
                    string physical_path1 = Configuration.RootPath + '\\' + "BadRequest.html";

                    content = File.ReadAllText(physical_path1);
                  
                     response = new Response(code, "text/html", content, physical_path1);
                    return response;
                }
               








                //TODO: map the relativeURI in request to get the physical path of the resource.
                string[] name = request.relativeURI.Split('/');
                string physical_path = Configuration.RootPath + '\\' + name[1];

                //TODO: check for redirect
                
                for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
                {
                    if ('/' + Configuration.RedirectionRules.Keys.ElementAt(i).ToString() == request.relativeURI)
                    {
                        code = "301";
                        request.relativeURI = '/' + Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                        name[1] = Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                        physical_path = Configuration.RootPath + '\\' + name[1];
                        content = File.ReadAllText(physical_path);
                        string Location = "http://localhost:1000/" + name[1];
                        content = File.ReadAllText(physical_path);
                        Response res = new Response(code, "text/html", content, Location);
                        return res;
                    }
                }

                //TODO: check file exists
                if (!File.Exists(physical_path))
                {
                    //TODO: read the physical file
                    physical_path = Configuration.RootPath + '\\' + "NotFound.html";
                    code = "404";
                    content = File.ReadAllText(physical_path);

                    //Console.WriteLine(Path.GetFullPath(path));
                }
                else
                {
                    content = File.ReadAllText(physical_path);
                    code = "200";
                }
                // Create OK response
                Response re = new Response(code, "text/html", content, physical_path);
                return re;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                string physical_path = Configuration.RootPath + '\\' + "InternalError.html";
                code = "500";
                content = File.ReadAllText(physical_path);
                Response re = new Response(code, "text/html", content, physical_path);
                return re;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (File.Exists(Configuration.RedirectionRules[relativePath]))
                return Configuration.RedirectionRules[relativePath];
            else
                return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string data = "";
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (File.Exists(filePath))
            {
                data = File.ReadAllText(filePath);
            }
            // else read file and return its content

            return data;
        }

        private void LoadRedirectionRules(string filePath)
        {
            Configuration.RedirectionRules=new Dictionary<string,string>();
            try
            {
                string path = @"E:\redirectionRules.txt";
                FileStream file = new FileStream(path, FileMode.Open);
                StreamReader newfile = new StreamReader(file);
                while (true)
                {
                    string data = newfile.ReadLine();
                    string[] retrieveddata = data.Split();
                    //Console.WriteLine(retrieveddata[0]);
                    if (retrieveddata[0] == "")
                       
                        break;
                    Configuration.RedirectionRules.Add(retrieveddata[0], retrieveddata[1]);
                  
                }
                
                file.Close();
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}