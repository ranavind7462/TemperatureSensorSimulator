using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace TemperatureSensorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //This is for selfhosting the Signalr server
            string url = "http://localhost:8077";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
            Console.ReadLine();
        }
    }
}
