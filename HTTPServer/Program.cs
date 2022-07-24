using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateRedirectionRulesFile();
            string filepath = @"E:\redirectionRules.txt";
            Server S1 = new Server(1000,filepath ); //object of server
            S1.StartServer();

        }

        static void CreateRedirectionRulesFile()
        {
            FileStream fs=new FileStream(@"E:\redirectionRules.txt",FileMode.Open,FileAccess.Write);
            StreamWriter streamWriter= new StreamWriter(fs);
           streamWriter.WriteLine(@"aboutus.html aboutus2.html");
           streamWriter.Close();
          
        }

    }
}