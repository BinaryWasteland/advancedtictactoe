using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;


namespace XtremeT3Server
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("XtremeT3Server.exe.config", false);

            Console.WriteLine("Xtreme Tic Tac Toe Server started. Press <Enter> to quit.");
            Console.ReadLine();
        }
    }
}
