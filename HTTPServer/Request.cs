using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        String method;
        public string relativeURI;

        public string HTTPVersion { get; private set; }

        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

       // HTTPVersion httpVersion;
        string requestString;//the whole http request
        //string[] contentLines;
        int j = 0;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
           
            string[] separators = new string[] { "\r\n" }; //each line is separated
            requestLines = requestString.Split(separators, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            //bool requestline_validation = ParseRequestLine();
            // Parse Request line 
            //request line contains three parts 1)method 2) uri 3)http version
            string[] line = requestLines[0].Split(' ');
            method = line[0];
            relativeURI = line[1];
            HTTPVersion = line[2];


            // Load header lines into HeaderLines dictionary
            int i = 1;
            
             headerLines=new Dictionary<string, string>();
            string[] separators2 = new string[] { ": " };
            while (!string.IsNullOrEmpty(requestLines[i]))
            {
                string header_line = requestLines[i];
                string[] dic_data = header_line.Split(separators2, StringSplitOptions.None);
                headerLines.Add(dic_data[0], dic_data[1]);
                i++;
                j = i; // this will be the index of the blank line for blank line check

               

            }

            if (!ValidateBlankLine())
                return false;

            if (!ValidateIsURI(relativeURI))
            
                return false;
            if (!Validaterequestline())
                return false;
            return true;
            

            
            //request line contains three parts 1)method 2) uri 3)http version
        }
        private bool Validaterequestline()
        {
            if (requestLines.Length <= 3)
                return false;
            if (method != "GET")
                return false;


          return true;
        }
        private bool ValidateBlankLine()
        {
            if (string.IsNullOrEmpty(requestLines[j]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
           
        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

    }
}


    


// Load header lines into HeaderLines dictionary


