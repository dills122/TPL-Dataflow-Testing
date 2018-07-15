using System;

namespace OrderProcessingPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Pipeline pipeline = new Pipeline();
            pipeline.StartPipeline();

            Console.ReadKey();
        }
    }
}
