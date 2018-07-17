using Newtonsoft.Json;
using StockReportPipeline.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Serialization;

namespace StockReportPipeline
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Pipeline pipeline = new Pipeline();

            Console.ReadKey();
        }
 
    }
}
