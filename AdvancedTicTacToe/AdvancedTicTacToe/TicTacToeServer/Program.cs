using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Http;

namespace TicTacToeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load the remoting config file
            RemotingConfiguration.Configure("TicTacToeServer.exe.config", false);

            // Keep the server running until <Enter> is pressed
            Console.WriteLine("TicTacToe Server started.  Press <Enter> to quit.");
            Console.ReadLine();
        }
    }
}
