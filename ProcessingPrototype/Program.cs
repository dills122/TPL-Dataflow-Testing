using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProcessingPrototype
{
    public class Program
    {
        static void Main(string[] args)
        {
            Processing processing = new Processing();
            Console.WriteLine("Hello World!");
            string FilePath = "";
            processing.Start().Wait();
            Console.ReadKey();
        }

    }
}
