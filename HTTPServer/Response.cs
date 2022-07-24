using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    
    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }

        public string Responsestring { get; internal set; }

       
        List<string> headerLines = new List<string>();
        public Response(String code, string contentType, string content, string redirectoinPath)
        {
// throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(DateTime.Now.ToString("ddd,dd MMM yyy HH:mm:ss 'Est'"));
            String status = GetStatusLine(code);
            if (status == "301")
            {
                headerLines.Add(redirectoinPath);
                responseString = status + "\r\n" + "Content-Type" + headerLines[0] + "\r\n" + "Content-Length" + headerLines[1] + "\r\n" + "Date" + headerLines[2] + "\r\n" + "location" + headerLines[3] + "\r\n" + "\r\n" + content;
            }
            else
            {
                responseString = status + "\r\n" + "Content-Type" + headerLines[0] + "\r\n" + "Content-Length" + headerLines[1] + "\r\n" + "Date" + headerLines[2]  + "\r\n" + "\r\n" + content;
            }

            // TODO: Create the request string

        }
        

        private string GetStatusLine(String code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            if (code == "200")
            {
                statusLine = "HTTP/1.1" + " " + code + " " + "OK";
            }
            else if (code == "301")
            {
                statusLine = "HTTP/1.1" + " " + code + " " + "Redirect";
            }
            else if (code == "400")
            {
                statusLine = "HTTP/1.1" + " " + code + " " + "Bad request";
            }
            else
            {
                statusLine = "HTTP/1.1" + " " + code + " " + "500 internal server";
            }
            return statusLine;
        }
    }
}
